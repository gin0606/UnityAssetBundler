using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;


class AssetBundler
{
    private string projectPath;
    private string resourceDirPath;
    private string outputDirPath;
    private BuildTarget[] buildTargets = new BuildTarget[] { };
    public BuildTarget[] BuildTargets
    {
        get
        {
            return buildTargets;
        }
        set
        {
            buildTargets = value ?? new BuildTarget[] { };
        }
    }
    private bool compress = false;
    public bool Compress
    {
        get
        {
            return compress;
        }
        set
        {
            compress = value;
        }
    }

    public AssetBundler(string projectPath, string resourcesDir = "Assets/Resources", string outputDir = "build")
    {
        this.projectPath = Path.GetFullPath(projectPath);
        this.resourceDirPath = Path.Combine(this.projectPath, resourcesDir);
        this.outputDirPath = Path.Combine(this.projectPath, outputDir);
    }

    public void build()
    {
        var bundleSets = Directory.GetDirectories(this.resourceDirPath)
            .Select(dir => Directory.GetDirectories(dir))
            .SelectMany(x => x)
            .Select(x => new BundleSet(x));
        foreach (var buildTarget in buildTargets)
        {
            foreach (var bundleSet in bundleSets)
            {
                bundleSet.bundle(buildTarget, this.resourceDirPath, this.outputDirPath, this.compress);
            }
        }
    }

    class BundleSet
    {
        private string resPath;
        private static string[] IGNORE_FILE_NAMES = new string[] {
            ".DS_Store",
        };
        public BundleSet(string root)
        {
            this.resPath = root;
        }

        public void bundle(BuildTarget buildTarget, string resourceDirPath, string outputDirPath, bool compress)
        {
            var bundleFileName = this.resPath.Split(Path.DirectorySeparatorChar).Last();
            var filePathWithoutExt = GetBundleFileRelativePathes(resPath)
                .Where(path => !IGNORE_FILE_NAMES.Contains(path))
                .Select(path => Path.GetDirectoryName(path) + Path.GetFileNameWithoutExtension(path))
                .ToArray();

            var basePath = resPath.GetRelativePathFrom(resourceDirPath);

            var assets = filePathWithoutExt.Select(path => Resources.Load(Path.Combine(basePath, path)));

            var outputDir = Path.Combine(outputDirPath, Path.Combine(basePath.Split(Path.DirectorySeparatorChar).First(), buildTarget.ToString()));
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var outPath = Path.Combine(outputDir, Path.ChangeExtension(bundleFileName, "unity3d"));

            var opt = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
            if (!compress)
            {
                opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
            }

            var result = BuildPipeline.BuildAssetBundleExplicitAssetNames(assets.ToArray(), filePathWithoutExt.ToArray(), outPath, opt, buildTarget);
        }
        private string[] GetBundleFileRelativePathes(string root)
        {
            var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
            var relativePathes = files
            .Where(path => !path.EndsWith(".meta"))
            .Select(path => path.GetRelativePathFrom(root))
            .ToArray();

            return relativePathes;
        }
    }
}

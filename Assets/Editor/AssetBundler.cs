using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;


class AssetBundler
{
    private string projectPath;
    private string resourceDirPath;
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
    private static string[] IGNORE_FILE_NAMES = new string[]{
        ".DS_Store",
    };
    private const string OUTPUT_BASE_DIR = "build";

    public AssetBundler(string projectPath)
    {
        this.projectPath = Path.GetFullPath(projectPath);
        this.resourceDirPath = Path.Combine(this.projectPath, "Assets/Resources");
    }

    public void build()
    {
        var directories = Directory.GetDirectories(this.resourceDirPath)
            .Select(dir => Directory.GetDirectories(dir))
            .SelectMany(x => x);
        foreach (var buildTarget in buildTargets)
        {
            foreach (var path in directories)
            {
                var name = path.Split(Path.DirectorySeparatorChar).Last();
                Export(buildTarget, path, name, !this.compress);
            }
        }
    }

    private bool Export(BuildTarget buildTarget, string resPath, string bundleFileName, bool uncompress)
    {
        var filePathWithoutExt = GetBundleFileRelativePathes(resPath)
            .Where(path => !IGNORE_FILE_NAMES.Contains(path))
            .Select(path => Path.GetDirectoryName(path) + Path.GetFileNameWithoutExtension(path))
            .ToArray();

        var basePath = resPath.GetRelativePathFrom(this.resourceDirPath);

        var assets = filePathWithoutExt.Select(path => Resources.Load(Path.Combine(basePath, path)));

        var outputDir = Path.Combine(OUTPUT_BASE_DIR, Path.Combine(basePath.Split(Path.DirectorySeparatorChar).First(), buildTarget.ToString()));
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        var outPath = Path.Combine(outputDir, Path.ChangeExtension(bundleFileName, "unity3d"));

        var opt = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
        if (uncompress)
        {
            opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
        }

        var result = BuildPipeline.BuildAssetBundleExplicitAssetNames(assets.ToArray(), filePathWithoutExt.ToArray(), outPath, opt, buildTarget);

        return result;
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

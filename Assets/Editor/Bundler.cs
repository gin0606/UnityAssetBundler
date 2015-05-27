using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

class Importer : UnityEditor.AssetPostprocessor
{
    public void OnPreprocessTexture()
    {
        Debug.LogError("assetPath: " + assetPath + "！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
    }
}

public class Bundler : MonoBehaviour
{
    const string PROJECT_PATH_OPTION_KEY = "-projectPath";
    const string UNCONPRESS_KEY = "-unconpress";

    const string RESOURCE_DIR_PATH = "Assets/Resources";
    private const string OUTPUT_BASE_DIR = "build";
    private static string[] IGNORE_FILE_NAMES = new string[]{
        ".DS_Store",
    };

    public static void ExportForAndroid()
    {
        ExportForTarget(BuildTarget.Android);
    }

    public static void ExportForiOS()
    {
        ExportForTarget(BuildTarget.iPhone);
    }

    public static void ExportForOSXUniversal()
    {
        ExportForTarget(BuildTarget.StandaloneOSXUniversal);
    }

    public static void ExportAll()
    {
        ExportForTarget(new BuildTarget[] { BuildTarget.Android, BuildTarget.iPhone, BuildTarget.StandaloneOSXUniversal });
    }

    private static void ExportForTarget(BuildTarget buildTarget)
    {
        ExportForTarget(new BuildTarget[] { buildTarget });
    }

    private static void ExportForTarget(BuildTarget[] buildTargets)
    {
        var args = System.Environment.GetCommandLineArgs();
        string projectPath = null;
        bool unconpress = false;
        for (int i = 0; i < args.Length; ++i)
        {
            switch (args[i])
            {
                case PROJECT_PATH_OPTION_KEY:
                    projectPath = args[i + 1];
                    break;
                case UNCONPRESS_KEY:
                    unconpress = true;
                    break;
            }
        }

        var projectResourceDirPath = Path.Combine(projectPath, RESOURCE_DIR_PATH);
        var directories = Directory.GetDirectories(projectResourceDirPath)
            .Select(dir => Directory.GetDirectories(dir))
            .SelectMany(x => x);

        foreach (var buildTarget in buildTargets)
        {
            foreach (var path in directories)
            {
                var name = path.Split(Path.DirectorySeparatorChar).Last();
                Export(buildTarget, path, name, unconpress);
            }
        }
        UnityEditor.EditorApplication.Exit(0);
    }

    public static bool Export(BuildTarget buildTarget, string resPath, string bundleFileName, bool uncompress)
    {
        var filePathAndExt = GetBundleFileRelativePathes(resPath)
            .Where(path => !IGNORE_FILE_NAMES.Contains(path))
            .ToDictionary(
                path => Path.GetDirectoryName(path) + Path.GetFileNameWithoutExtension(path),
                path => Path.GetExtension(path)
            );

        var basePath = resPath.GetRelativePathFrom(Path.GetFullPath(RESOURCE_DIR_PATH));

        var assets = filePathAndExt.Keys.Select(path => Resources.Load(Path.Combine(basePath, path)));

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

        var result = BuildPipeline.BuildAssetBundleExplicitAssetNames(assets.ToArray(), filePathAndExt.Keys.ToArray(), outPath, opt, buildTarget);

        return result;
    }

    private static string[] GetBundleFileRelativePathes(string root)
    {
        var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
        var relativePathes = files
        .Where(path => !path.EndsWith(".meta"))
        .Select(path => path.GetRelativePathFrom(root))
        .ToArray();

        return relativePathes;
    }
}

static class PathExtention
{
    public static string GetRelativePathFrom(this string toPath, string fromPath)
    {
        var toURI = new System.Uri(toPath);
        var fromURI = new System.Uri(fromPath + Path.DirectorySeparatorChar);

        var relativeURI = fromURI.MakeRelativeUri(toURI);
        return System.Uri.UnescapeDataString(relativeURI.ToString());
    }
}

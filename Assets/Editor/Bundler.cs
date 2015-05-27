using UnityEngine;
using UnityEditor;


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

        var assetBundler = new AssetBundler(projectPath);
        assetBundler.BuildTargets = buildTargets;
        assetBundler.Compress = !unconpress;
        assetBundler.build();
        UnityEditor.EditorApplication.Exit(0);
    }
}

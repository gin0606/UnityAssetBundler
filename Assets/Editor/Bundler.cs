using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class Bundler : MonoBehaviour {
	const string PROJECT_PATH_OPTION_KEY = "-projectPath";
	const string UNCONPRESS_KEY = "-unconpress";

	const string RESOURCE_DIR_PATH = "Assets/Resources";
	private const string OUTPUT_BASE_DIR = "build";
	private static string[] IGNORE_FILE_NAMES = new string[]{
		".DS_Store",
	};

	public static void ExportAll()
	{
		var args = System.Environment.GetCommandLineArgs();
		string projectPath = null;
		bool unconpress = false;
		for(int i = 0; i < args.Length; ++i) {
			switch(args[i]){
				case PROJECT_PATH_OPTION_KEY:
					projectPath = args[i + 1];
					break;
				case UNCONPRESS_KEY:
					unconpress = true;
					break;
			}
		}

		var directories = Directory.GetDirectories(Path.Combine(projectPath, RESOURCE_DIR_PATH));
		var subDirectories = directories.Select(dir => Directory.GetDirectories(dir)).SelectMany(x => x);

		foreach(var path in subDirectories){
			var name = path.Split(Path.DirectorySeparatorChar).Last();
			Export(BuildTarget.Android, path, name, unconpress);
			Export(BuildTarget.iPhone, path, name, unconpress);
			Export(BuildTarget.StandaloneOSXUniversal, path, name, unconpress);
		}
		UnityEditor.EditorApplication.Exit(0);
	}

	public static bool Export (BuildTarget buildTarget, string resPath, string bundleFileName, bool uncompress)
	{
		var filePathAndExt = GetFileRelativePathes(resPath)
			.Where(path => !IGNORE_FILE_NAMES.Contains(path))
			.ToDictionary(
				path => Path.GetDirectoryName(path) + Path.GetFileNameWithoutExtension(path),
				path => Path.GetExtension(path)
			);

		var startIndex = resPath.LastIndexOf(RESOURCE_DIR_PATH) + RESOURCE_DIR_PATH.Length;
		var basePath = resPath.Substring(startIndex);
		if (basePath.StartsWith(Path.DirectorySeparatorChar.ToString())) {
			basePath = basePath.Substring(1);
		}

		var assets = filePathAndExt.Keys.Select(path => Resources.Load (Path.Combine(basePath, path)));

		var outputDir = Path.Combine(OUTPUT_BASE_DIR, Path.Combine(basePath.Split(Path.DirectorySeparatorChar).First(), buildTarget.ToString()));
		if (!Directory.Exists (outputDir)) {
			Directory.CreateDirectory (outputDir);
		}

		var outPath = Path.Combine(outputDir, Path.ChangeExtension(bundleFileName, "unity3d"));

		var opt = BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets;
		if (uncompress) {
			opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
		}

		var result = BuildPipeline.BuildAssetBundleExplicitAssetNames (assets.ToArray(), filePathAndExt.Keys.ToArray(), outPath, opt, buildTarget);

		return result;
	}

	private static string[] GetFileRelativePathes(string root)
	{
		var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
		var relativePathes = files
		.Where(path => !path.EndsWith(".meta"))
		.Select(path => {
			// remove(root + "/")
			var relativePath = path.Substring(path.LastIndexOf(root) + root.Length).Substring(1);
			return relativePath;
		}).ToArray();
		return relativePathes;
	}
}

using System.IO;

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

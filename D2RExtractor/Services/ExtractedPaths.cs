using System.IO;

namespace D2RExtractor.Services;

/// <summary>
/// Converts between the backslash-separated relative paths that storage and manifests use and real
/// paths on disk. Outside Windows a backslash is an ordinary filename character, so skipping this
/// writes every file flat into one folder and makes manifests incomparable with the disk.
/// </summary>
internal static class ExtractedPaths
{
    public static string ToDisk(string root, string relPath) =>
        Path.Combine(root, relPath.Replace('\\', Path.DirectorySeparatorChar));

    public static string FromDisk(string root, string fullPath) =>
        Path.GetRelativePath(root, fullPath).Replace(Path.DirectorySeparatorChar, '\\');
}

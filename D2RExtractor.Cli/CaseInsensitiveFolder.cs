using System.Runtime.InteropServices;

namespace D2RExtractor.Cli;

/// <summary>
/// Creates folders whose name lookups ignore case, where the filesystem supports it (ext4 with the
/// casefold feature, as on the Steam Deck). The game asks for the same file in different casings than
/// the archives record. Under Proton every mismatch otherwise costs a scan of the whole folder; in a
/// case-folding folder the first lookup simply succeeds. Subfolders inherit the flag.
/// </summary>
internal static class CaseInsensitiveFolder
{
    private const int ODirectory = 0x10000;
    private const uint FsIocGetFlags = 0x80086601;
    private const uint FsIocSetFlags = 0x40086602;
    private const int FsCasefoldFl = 0x40000000;

    /// <summary>
    /// Creates <paramref name="path"/> and marks it case-insensitive. Returns false when the folder
    /// could not be marked; the folder exists either way.
    /// </summary>
    public static bool TryCreate(string path)
    {
        Directory.CreateDirectory(path);
        if (!OperatingSystem.IsLinux())
            return false;

        int fd = open(path, ODirectory);
        if (fd < 0)
            return false;
        try
        {
            int flags = 0;
            if (ioctl(fd, FsIocGetFlags, ref flags) != 0)
                return false;
            flags |= FsCasefoldFl;
            return ioctl(fd, FsIocSetFlags, ref flags) == 0;
        }
        finally
        {
            close(fd);
        }
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int open(string path, int flags);

    [DllImport("libc", SetLastError = true)]
    private static extern int ioctl(int fd, uint request, ref int arg);

    [DllImport("libc")]
    private static extern int close(int fd);
}

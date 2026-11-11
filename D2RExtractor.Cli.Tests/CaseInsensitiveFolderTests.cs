using Shouldly;
using Xunit;

namespace D2RExtractor.Cli.Tests;

public sealed class CaseInsensitiveFolderTests : IDisposable
{
    private readonly string _root = Directory.CreateTempSubdirectory("d2r-casefold-").FullName;

    public void Dispose() => Directory.Delete(_root, recursive: true);

    /// <summary>
    /// Most filesystems cannot fold case (the test machine's included), and extraction must carry on
    /// regardless: the flag only saves Wine some directory scans.
    /// </summary>
    [Fact]
    public void CreatesTheFolderEvenWhereCaseFoldingIsUnsupported()
    {
        string folder = Path.Combine(_root, "global");

        Should.NotThrow(() => CaseInsensitiveFolder.TryCreate(folder));

        Directory.Exists(folder).ShouldBeTrue();
    }
}

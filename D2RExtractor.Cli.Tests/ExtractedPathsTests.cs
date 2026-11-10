using D2RExtractor.Services;
using Shouldly;
using Xunit;

namespace D2RExtractor.Cli.Tests;

/// <summary>
/// Manifests and storage paths always use backslashes. On Linux a backslash is an ordinary filename
/// character, so these conversions are what keep the extracted tree nested and the manifest
/// comparable to what is on disk.
/// </summary>
public class ExtractedPathsTests
{
    private static readonly string Root = Path.Combine(Path.GetTempPath(), "d2r");

    [Fact]
    public void ARecordedPathBecomesANestedFile()
    {
        ExtractedPaths.ToDisk(Root, @"data\global\excel\armor.txt")
            .ShouldBe(Path.Combine(Root, "data", "global", "excel", "armor.txt"));
    }

    [Fact]
    public void AFileOnDiskBecomesTheRecordedForm()
    {
        ExtractedPaths.FromDisk(Root, Path.Combine(Root, "data", "global", "excel", "armor.txt"))
            .ShouldBe(@"data\global\excel\armor.txt");
    }
}

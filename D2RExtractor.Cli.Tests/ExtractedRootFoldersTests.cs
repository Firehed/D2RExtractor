using D2RExtractor.Services;
using Shouldly;
using Xunit;

namespace D2RExtractor.Cli.Tests;

public class ExtractedRootFoldersTests
{
    [Fact]
    public void TheExtractedTreeHasThreeRoots()
    {
        CascExtractorService.ExtractedRootFolders.ShouldBe([@"data\global", @"data\hd", @"data\local"]);
    }
}

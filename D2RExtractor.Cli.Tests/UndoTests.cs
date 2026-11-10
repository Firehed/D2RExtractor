using D2RExtractor.Models;
using D2RExtractor.Services;
using Shouldly;
using Xunit;

namespace D2RExtractor.Cli.Tests;

public sealed class UndoTests : IDisposable
{
    private readonly string _root = Directory.CreateTempSubdirectory("d2r-undo-").FullName;

    public void Dispose() => Directory.Delete(_root, recursive: true);

    [Fact]
    public void UndoRemovesFilesRecordedWithBackslashes()
    {
        string extracted = Path.Combine(_root, "data", "global", "excel", "armor.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(extracted)!);
        File.WriteAllText(extracted, "x");

        var target = new ExtractionTarget { FolderPath = _root };
        var manifest = new ExtractionManifest { ManifestVersion = ExtractionManifest.CurrentVersion };
        ManifestService.SaveManifest(target, manifest);
        ManifestService.WriteAllEntries(target, manifest, [new ManifestEntry(@"data\global\excel\armor.txt", null, 1)]);
        ManifestService.SaveManifest(target, manifest);

        new CascExtractorService().UndoExtraction(target, null, null, CancellationToken.None);

        File.Exists(extracted).ShouldBeFalse();
        Directory.Exists(Path.Combine(_root, "data", "global")).ShouldBeFalse();
    }
}

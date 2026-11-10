using D2RExtractor.Models;
using D2RExtractor.Services;
using D2RExtractor.Services.Steam;

if (args.Length != 2 || args[0] is not ("extract" or "undo"))
{
    Console.Error.WriteLine("Usage: d2rextract <extract|undo> <game-folder>");
    Console.Error.WriteLine("  extract  Extract the archives, or update an existing extraction.");
    Console.Error.WriteLine("  undo     Remove every file a previous extraction wrote.");
    return 2;
}

string folder = Path.GetFullPath(args[1]);

// Battle.net installs need CascLib, a Windows-only native library this build does not ship.
if (!SteamStaticStorage.IsSteamStaticStorage(folder))
{
    Console.Error.WriteLine($"Not a Steam D2R install (no data/.build.config): {folder}");
    return 1;
}

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

// Progress redraws one line in place; log messages clear it first so the two do not interleave.
void Log(string message) => Console.WriteLine($"\r\x1b[K{message}");
var progress = new ConsoleProgress();

var installation = new D2RInstallation { FolderPath = folder };
var target = new ExtractionTarget { FolderPath = folder };
var service = new CascExtractorService();

try
{
    if (args[0] == "undo")
    {
        service.UndoExtraction(target, progress, Log, cts.Token);
    }
    else if (ManifestService.LoadManifest(target) is { } manifest)
    {
        service.UpdateExtraction(installation, target, manifest, false, null, false, progress, Log, cts.Token);
    }
    else
    {
        service.Extract(installation, target, false, null, progress, Log, cts.Token);
    }
}
catch (OperationCanceledException)
{
    Log("Cancelled. Run extract again to resume.");
    return 130;
}
catch (Exception ex) when (ex is InvalidOperationException or IOException or UnauthorizedAccessException)
{
    Log($"Error: {ex.Message}");
    return 1;
}

return 0;

/// <summary>
/// Reports on the calling thread. <see cref="Progress{T}"/> would post to the thread pool here,
/// letting a stale progress line land after, or in the middle of, a log message.
/// </summary>
sealed class ConsoleProgress : IProgress<ExtractionProgress>
{
    public void Report(ExtractionProgress p) =>
        Console.Write(p.TotalFiles > 0
            ? $"\r\x1b[K{p.Phase} {p.FilesProcessed:N0}/{p.TotalFiles:N0}"
            : $"\r\x1b[K{p.Phase}…");
}

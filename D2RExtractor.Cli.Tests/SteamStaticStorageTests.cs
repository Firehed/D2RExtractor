using D2RExtractor.Services.Steam;
using Shouldly;
using Xunit;

namespace D2RExtractor.Cli.Tests;

public class SteamStaticStorageTests
{
    /// <summary>
    /// The game asks for files in this casing. Under Proton, a name that differs only in case still
    /// opens, but each lookup costs a scan of the whole folder instead of a single stat.
    /// </summary>
    [Fact]
    public void CanonicalPathsKeepTheirCase()
    {
        SteamStaticStorage.ToVirtualPath("data:data/hd/global/textures/smaa/SMAA_Texture.dds")
            .ShouldBe(@"data:data\hd\global\textures\smaa\SMAA_Texture.dds");
    }
}

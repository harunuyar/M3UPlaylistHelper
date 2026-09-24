namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Epg;
using M3UPlaylistHelper.Parser;
using System.IO.Compression;
using System.Text;

public class EpgTests
{
    private const string Guide = """
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE tv SYSTEM "xmltv.dtd">
        <tv generator-info-name="test">
          <channel id="bbc1.uk">
            <display-name>BBC One</display-name>
            <display-name>BBC 1</display-name>
            <icon src="http://logo" />
          </channel>
          <channel id="cnn.us"><display-name>CNN International</display-name></channel>
          <channel id="dup1"><display-name>Same Name</display-name></channel>
          <channel id="dup2"><display-name>Same Name</display-name></channel>
          <programme start="20260101000000 +0000" channel="bbc1.uk"><title>News</title></programme>
          <channel id="after.programmes"><display-name>Ignored</display-name></channel>
        </tv>
        """;

    private static Task<XmlTvGuide> LoadAsync(byte[] bytes) =>
        XmlTvGuide.LoadAsync(new MemoryStream(bytes), CancellationToken.None);

    [Fact]
    public async Task ReadsChannelsUntilFirstProgramme()
    {
        var guide = await LoadAsync(Encoding.UTF8.GetBytes(Guide));

        Assert.Equal(4, guide.ChannelCount);
        Assert.True(guide.ContainsId("BBC1.UK"));
        Assert.False(guide.ContainsId("after.programmes"));
        Assert.Equal("bbc1.uk", guide.FindIdByName("BBC 1"));
        Assert.Null(guide.FindIdByName("Same Name"));
    }

    [Fact]
    public async Task ReadsGzippedGuide()
    {
        var compressed = new MemoryStream();
        using (var gzip = new GZipStream(compressed, CompressionMode.Compress, leaveOpen: true))
        {
            gzip.Write(Encoding.UTF8.GetBytes(Guide));
        }

        var guide = await LoadAsync(compressed.ToArray());

        Assert.True(guide.ContainsId("cnn.us"));
    }

    [Theory]
    [InlineData("UK: BBC One HD", "bbcone")]
    [InlineData("|UK| BBC ONE FHD", "bbcone")]
    [InlineData("BBC One", "bbcone")]
    [InlineData("[DE] Das Erste", "daserste")]
    [InlineData("ESPN: Live", "espnlive")]
    [InlineData("HD", "hd")]
    public void NormalizesNames(string name, string expected)
    {
        Assert.Equal(expected, XmlTvGuide.NormalizeName(name));
    }

    [Fact]
    public async Task FillsMissingTvgIdsAndExcludesChannelsWithoutGuide()
    {
        var guide = await LoadAsync(Encoding.UTF8.GetBytes(Guide));
        var playlist = M3UParser.Parse("""
            #EXTINF:-1 tvg-id="cnn.us",CNN
            http://x/cnn
            #EXTINF:-1 tvg-name="UK: BBC One HD",BBC One
            http://x/bbc
            #EXTINF:-1,Unknown Channel
            http://x/unknown
            """);

        Assert.Equal(1, guide.FillMissingTvgIds(playlist));
        Assert.Equal("bbc1.uk", playlist.AllChannels.ElementAt(1).TvgId);

        Assert.Equal(1, guide.ExcludeChannelsWithoutGuide(playlist));
        Assert.Equal(["CNN", "BBC One"], playlist.ExportedChannels.Select(c => c.Name));
    }
}

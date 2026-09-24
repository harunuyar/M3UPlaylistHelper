namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Parser;

public class M3UWriterTests
{
    private const string Source = """
        #EXTM3U url-tvg="http://epg/guide.xml"
        #EXTINF:-1 tvg-id="a" tvg-chno="1" tvg-logo="http://logo/a.png" group-title="News",Channel A
        #EXTVLCOPT:http-referrer=http://example.com
        http://example.com/a
        #EXTINF:-1 tvg-id="b" group-title="News",Channel B
        http://example.com/b
        #EXTINF:-1 tvg-id="c" group-title="Movies",Channel C
        http://example.com/c
        """;

    [Fact]
    public void RoundTripsAllData()
    {
        var output = M3UWriter.Write(M3UParser.Parse(Source));

        Assert.Equal(
            """
            #EXTM3U url-tvg="http://epg/guide.xml"
            #EXTINF:-1 tvg-id="a" tvg-chno="1" tvg-logo="http://logo/a.png" group-title="News",Channel A
            #EXTVLCOPT:http-referrer=http://example.com
            http://example.com/a
            #EXTINF:-1 tvg-id="b" group-title="News",Channel B
            http://example.com/b
            #EXTINF:-1 tvg-id="c" group-title="Movies",Channel C
            http://example.com/c

            """.ReplaceLineEndings("\n"),
            output);
    }

    [Fact]
    public void SkipsExcludedItemsAndUsesRenamedCategoryTitle()
    {
        var playlist = M3UParser.Parse(Source);
        playlist.Categories[0].Title = "World News";
        playlist.Categories[0].Channels[1].IsIncluded = false;
        playlist.Categories[1].IsIncluded = false;

        var reparsed = M3UParser.Parse(M3UWriter.Write(playlist));

        var category = Assert.Single(reparsed.Categories);
        Assert.Equal("World News", category.Title);
        Assert.Equal("Channel A", Assert.Single(category.Channels).Name);
    }

    [Fact]
    public async Task WritesFile()
    {
        var file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.m3u");

        try
        {
            await M3UWriter.WriteFileAsync(M3UParser.Parse(Source), file, CancellationToken.None);
            var reparsed = await M3UParser.ParseFileAsync(file, CancellationToken.None);

            Assert.Equal(3, reparsed.AllChannels.Count());
            Assert.False(File.Exists(file + ".tmp"));
        }
        finally
        {
            File.Delete(file);
        }
    }
}

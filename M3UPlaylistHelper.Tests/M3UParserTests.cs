namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Parser;

public class M3UParserTests
{
    [Fact]
    public void ParsesBasicEntries()
    {
        var playlist = M3UParser.Parse("""
            #EXTM3U
            #EXTINF:-1 tvg-id="bbc1.uk" tvg-name="BBC One" tvg-logo="http://logo/bbc1.png" group-title="UK",BBC One HD
            http://example.com/bbc1
            #EXTINF:-1 tvg-id="cnn.us" group-title="News",CNN
            http://example.com/cnn
            #EXTINF:-1 tvg-id="bbc2.uk" group-title="UK",BBC Two
            http://example.com/bbc2
            """);

        Assert.Equal(["UK", "News"], playlist.Categories.Select(c => c.Title));
        Assert.Equal(2, playlist.Categories[0].Channels.Count);

        var bbc1 = playlist.Categories[0].Channels[0];
        Assert.Equal("BBC One HD", bbc1.Name);
        Assert.Equal("http://example.com/bbc1", bbc1.Url);
        Assert.Equal("bbc1.uk", bbc1.TvgId);
        Assert.Equal("BBC One", bbc1.TvgName);
        Assert.Equal("http://logo/bbc1.png", bbc1.LogoUrl);
        Assert.Same(playlist.Categories[0], bbc1.Category);
    }

    [Fact]
    public void KeepsCommasInNamesAndAttributes()
    {
        var playlist = M3UParser.Parse("""
            #EXTM3U
            #EXTINF:-1 tvg-name="Sky Sports, Main Event" group-title="Sports, UK",Sky Sports, Main Event
            http://example.com/sky
            """);

        var category = Assert.Single(playlist.Categories);
        Assert.Equal("Sports, UK", category.Title);
        Assert.Equal("Sky Sports, Main Event", category.Channels[0].Name);
        Assert.Equal("Sky Sports, Main Event", category.Channels[0].TvgName);
    }

    [Fact]
    public void AttachesDirectiveLinesAndAcceptsNonHttpUrls()
    {
        var playlist = M3UParser.Parse("""
            #EXTM3U
            #EXTINF:-1 group-title="Radio",Radio 1
            #EXTVLCOPT:http-user-agent=Mozilla
            #KODIPROP:inputstream=inputstream.adaptive
            rtmp://example.com/live/radio1
            #EXTINF:-1,Local File
            C:\Videos\clip.mp4
            """);

        var radio = playlist.Categories[0].Channels[0];
        Assert.Equal("rtmp://example.com/live/radio1", radio.Url);
        Assert.Equal(["#EXTVLCOPT:http-user-agent=Mozilla", "#KODIPROP:inputstream=inputstream.adaptive"], radio.ExtraLines);

        var local = playlist.Categories[1].Channels[0];
        Assert.Equal(M3UParser.DefaultCategory, local.Category.Title);
        Assert.Equal(@"C:\Videos\clip.mp4", local.Url);
    }

    [Fact]
    public void UsesExtGrpWhenGroupTitleIsMissing()
    {
        var playlist = M3UParser.Parse("""
            #EXTM3U
            #EXTINF:-1,Channel A
            #EXTGRP:Movies
            http://example.com/a
            #EXTINF:-1,Channel B
            http://example.com/b
            """);

        Assert.Equal(["Movies", M3UParser.DefaultCategory], playlist.Categories.Select(c => c.Title));
    }

    [Fact]
    public void HandlesBomWindowsLineEndingsAndUrlsWithoutExtInf()
    {
        var playlist = M3UParser.Parse("\uFEFF#EXTM3U\r\n\r\nhttp://example.com/plain\r\n");

        var channel = Assert.Single(playlist.AllChannels);
        Assert.Equal("http://example.com/plain", channel.Name);
        Assert.Equal("http://example.com/plain", channel.Url);
    }

    [Fact]
    public void ParsesHeaderAttributes()
    {
        var playlist = M3UParser.Parse("#EXTM3U url-tvg=\"http://epg/guide.xml\" tvg-shift=2\n");

        Assert.Equal(
            [new("url-tvg", "http://epg/guide.xml"), new("tvg-shift", "2")],
            playlist.HeaderAttributes);
    }

    [Fact]
    public void ParsesUnquotedAttributesAndDuration()
    {
        M3UParser.ParseExtInf("#EXTINF:0 tvg-chno=5 catchup=\"default\",Five", out var duration, out var attributes, out var name);

        Assert.Equal("0", duration);
        Assert.Equal([new("tvg-chno", "5"), new("catchup", "default")], attributes);
        Assert.Equal("Five", name);
    }
}

namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Parser;
using M3UPlaylistHelper.Tools;

public class MergeAndMoveTests
{
    private const string First = """
        #EXTM3U url-tvg="http://epg/a.xml" tvg-shift="1"
        #EXTINF:-1 group-title="News",A1
        http://a/1
        #EXTINF:-1 group-title="News",A2
        http://a/2
        #EXTINF:-1 group-title="News",A3
        http://a/3
        #EXTINF:-1 group-title="Sports",A4
        http://a/4
        """;

    private const string Second = """
        #EXTM3U x-tvg-url="http://epg/b.xml,http://epg/a.xml" tvg-shift="5"
        #EXTINF:-1 group-title="news",B1
        http://b/1
        #EXTINF:-1 group-title="Movies",B2
        http://b/2
        """;

    [Fact]
    public void MergeCombinesCategoriesAndEpgUrls()
    {
        var target = M3UParser.Parse(First);

        Assert.Equal(2, PlaylistTools.Merge(target, M3UParser.Parse(Second)));

        Assert.Equal(["News", "Sports", "Movies"], target.Categories.Select(c => c.Title));
        Assert.Equal(["A1", "A2", "A3", "B1"], target.Categories[0].Channels.Select(c => c.Name));
        Assert.Same(target.Categories[0], target.Categories[0].Channels[3].Category);
        Assert.Equal(["http://epg/a.xml", "http://epg/b.xml"], target.EpgUrls);
        Assert.Equal("1", target.HeaderAttributes.Single(a => a.Key == "tvg-shift").Value);
    }

    [Fact]
    public void MovesChannelsWithinCategory()
    {
        var playlist = M3UParser.Parse(First);
        var news = playlist.Categories[0];

        Assert.True(PlaylistTools.MoveChannels(playlist, [news.Channels[2]], news, news.Channels[0]));
        Assert.Equal(["A3", "A1", "A2"], news.Channels.Select(c => c.Name));

        Assert.True(PlaylistTools.MoveChannels(playlist, [news.Channels[0]], news, null));
        Assert.Equal(["A1", "A2", "A3"], news.Channels.Select(c => c.Name));

        Assert.False(PlaylistTools.MoveChannels(playlist, [news.Channels[0]], news, news.Channels[0]));
    }

    [Fact]
    public void MovesChannelsToOtherCategoryAndRemovesEmptyOnes()
    {
        var playlist = M3UParser.Parse(First);
        var news = playlist.Categories[0];
        var sports = playlist.Categories[1];

        PlaylistTools.MoveChannels(playlist, [sports.Channels[0]], news, news.Channels[1]);

        Assert.Equal(["A1", "A4", "A2", "A3"], news.Channels.Select(c => c.Name));
        Assert.Same(news, news.Channels[1].Category);
        Assert.Equal(["News"], playlist.Categories.Select(c => c.Title));
    }

    [Fact]
    public void MovesChannelsToNewCategory()
    {
        var playlist = M3UParser.Parse(First);
        var news = playlist.Categories[0];
        var favorites = new M3UPlaylistHelper.Model.Category("Favorites");

        PlaylistTools.MoveChannels(playlist, [news.Channels[0], news.Channels[2]], favorites, null);

        Assert.Equal(["News", "Sports", "Favorites"], playlist.Categories.Select(c => c.Title));
        Assert.Equal(["A1", "A3"], favorites.Channels.Select(c => c.Name));
        Assert.Contains("group-title=\"Favorites\",A3", M3UWriter.Write(playlist));
    }

    [Fact]
    public void MovesCategoryBeforeAnother()
    {
        var playlist = M3UParser.Parse(First + """

            #EXTINF:-1 group-title="Movies",M1
            http://m/1
            """);

        PlaylistTools.MoveCategoryBefore(playlist, playlist.Categories[2], playlist.Categories[0]);
        Assert.Equal(["Movies", "News", "Sports"], playlist.Categories.Select(c => c.Title));

        PlaylistTools.MoveCategoryBefore(playlist, playlist.Categories[0], null);
        Assert.Equal(["News", "Sports", "Movies"], playlist.Categories.Select(c => c.Title));
    }
}

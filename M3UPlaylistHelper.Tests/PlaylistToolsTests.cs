namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Parser;
using M3UPlaylistHelper.Tools;

public class PlaylistToolsTests
{
    private const string Source = """
        #EXTM3U
        #EXTINF:-1 group-title="Sports",ESPN
        http://example.com/espn
        #EXTINF:-1 group-title="Sports",Eurosport
        http://example.com/eurosport
        #EXTINF:-1 group-title="Adult",Hidden
        http://example.com/hidden
        #EXTINF:-1 group-title="All",ESPN (copy)
        http://example.com/espn
        """;

    [Fact]
    public void ExcludesDuplicateUrls()
    {
        var playlist = M3UParser.Parse(Source);

        Assert.Equal(1, PlaylistTools.ExcludeDuplicateChannels(playlist));
        Assert.False(playlist.Categories[2].Channels[0].IsIncluded);
        Assert.True(playlist.Categories[0].Channels[0].IsIncluded);
    }

    [Fact]
    public void SortsAndMovesCategories()
    {
        var playlist = M3UParser.Parse(Source);

        PlaylistTools.SortCategories(playlist);
        Assert.Equal(["Adult", "All", "Sports"], playlist.Categories.Select(c => c.Title));

        Assert.Equal(0, PlaylistTools.MoveCategory(playlist, playlist.Categories[2], -5));
        Assert.Equal(["Sports", "Adult", "All"], playlist.Categories.Select(c => c.Title));
    }

    [Fact]
    public void SelectionProfileReappliesToUpdatedPlaylist()
    {
        var playlist = M3UParser.Parse(Source);
        playlist.Categories[1].IsIncluded = false; // Adult
        playlist.Categories[0].Channels[1].IsIncluded = false; // Eurosport

        var profile = SelectionProfile.Capture(playlist, includeNewCategories: false);

        // The provider updated the playlist: a new category and a new channel appeared
        var updated = M3UParser.Parse(Source + """

            #EXTINF:-1 group-title="Sports",Sky Sports
            http://example.com/sky
            #EXTINF:-1 group-title="Shopping",QVC
            http://example.com/qvc
            """);

        profile.Apply(updated);

        Assert.Equal(["ESPN", "Sky Sports"], updated.ExportedChannels.Where(c => c.Category.Title == "Sports").Select(c => c.Name));
        Assert.False(updated.Categories.Single(c => c.Title == "Adult").IsIncluded);
        Assert.False(updated.Categories.Single(c => c.Title == "Shopping").IsIncluded);
        Assert.True(updated.Categories.Single(c => c.Title == "All").IsIncluded);
    }

    [Fact]
    public async Task SelectionProfileSurvivesSaveAndLoad()
    {
        var playlist = M3UParser.Parse(Source);
        playlist.Categories[1].IsIncluded = false;
        var file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

        try
        {
            await SelectionProfile.Capture(playlist, includeNewCategories: true).SaveAsync(file, CancellationToken.None);
            var loaded = await SelectionProfile.LoadAsync(file, CancellationToken.None);

            Assert.Equal(["Adult"], loaded.ExcludedCategories);
            Assert.True(loaded.IncludeNewCategories);
        }
        finally
        {
            File.Delete(file);
        }
    }
}

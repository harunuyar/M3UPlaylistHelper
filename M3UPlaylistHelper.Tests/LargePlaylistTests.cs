namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Parser;
using M3UPlaylistHelper.Tools;
using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

/// <summary>
/// Big IPTV playlists (VOD included) easily have hundreds of thousands of entries and thousands of categories.
/// These tests catch accidentally quadratic code: the limits are generous, a quadratic algorithm takes minutes.
/// </summary>
public class LargePlaylistTests(ITestOutputHelper output)
{
    private const int ChannelCount = 300_000;
    private const int CategoryCount = 3_000;

    private static string CreatePlaylist(string prefix = "")
    {
        var sb = new StringBuilder("#EXTM3U url-tvg=\"http://epg/guide.xml\"\n");

        for (int i = 0; i < ChannelCount; i++)
        {
            int category = i % CategoryCount;
            sb.Append($"#EXTINF:-1 tvg-id=\"ch{i}.{prefix}\" tvg-name=\"{prefix}Channel {i}\" tvg-logo=\"http://logo/{i}.png\" group-title=\"{prefix}Category {category}\",{prefix}Channel {i} HD\n");
            sb.Append($"http://stream.example.com/live/user/pass/{prefix}{i}.ts\n");
        }

        return sb.ToString();
    }

    private T Measure<T>(string name, Func<T> action, int maxMilliseconds)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = action();
        output.WriteLine($"{name}: {stopwatch.ElapsedMilliseconds} ms");
        Assert.True(stopwatch.ElapsedMilliseconds < maxMilliseconds, $"{name} took {stopwatch.ElapsedMilliseconds} ms");
        return result;
    }

    [Fact]
    public void HandlesLargePlaylists()
    {
        var content = CreatePlaylist();
        var playlist = Measure("Parse", () => M3UParser.Parse(content), 10_000);

        Assert.Equal(CategoryCount, playlist.Categories.Count);
        Assert.Equal(ChannelCount, playlist.AllChannels.Count());

        var matches = Measure("Search all channels", () => playlist.AllChannels.Where(c => PlaylistTools.Matches(c, "channel 12345 hd")).ToList(), 3_000);
        Assert.Single(matches);

        Measure("Count exported", () => playlist.ExportedChannels.Count(), 1_000);

        // Move a tenth of all channels into one category, and every channel of a category to the end of another
        var toMove = playlist.AllChannels.Where((_, i) => i % 10 == 0).ToList();
        Measure("Move 30k channels", () => PlaylistTools.MoveChannels(playlist, toMove, playlist.Categories[1], playlist.Categories[1].Channels[0]), 3_000);
        Assert.Equal(ChannelCount, playlist.AllChannels.Count());

        Measure("Exclude duplicates", () => PlaylistTools.ExcludeDuplicateChannels(playlist), 3_000);

        var other = M3UParser.Parse(CreatePlaylist("B"));
        Measure("Merge", () => PlaylistTools.Merge(playlist, other), 3_000);
        Assert.Equal(ChannelCount * 2, playlist.AllChannels.Count());

        var written = Measure("Write", () => M3UWriter.Write(playlist), 10_000);
        Assert.True(written.Length > content.Length);

        var profile = SelectionProfile.Capture(playlist, includeNewCategories: true);
        Measure("Apply profile", () => { profile.Apply(playlist); return 0; }, 3_000);
    }
}

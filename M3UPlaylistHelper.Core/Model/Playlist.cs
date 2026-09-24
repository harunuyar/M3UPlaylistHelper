namespace M3UPlaylistHelper.Model;

public class Playlist
{
    public const string UrlTvgAttribute = "url-tvg";

    /// <summary>
    /// Attributes of the #EXTM3U header line, such as url-tvg / x-tvg-url (EPG sources). Preserved when saving.
    /// </summary>
    public List<KeyValuePair<string, string>> HeaderAttributes { get; set; } = [];

    public List<Category> Categories { get; set; } = [];

    public IEnumerable<Channel> AllChannels => Categories.SelectMany(c => c.Channels);

    /// <summary>
    /// Channels that will be written when the playlist is saved.
    /// </summary>
    public IEnumerable<Channel> ExportedChannels =>
        Categories.Where(c => c.IsIncluded).SelectMany(c => c.Channels).Where(ch => ch.IsIncluded);

    /// <summary>
    /// EPG (XMLTV) URLs listed in the header. Both url-tvg and x-tvg-url are used in the wild, and both may hold
    /// several comma separated URLs.
    /// </summary>
    public IReadOnlyList<string> EpgUrls =>
        HeaderAttributes
            .Where(a => IsEpgAttribute(a.Key))
            .SelectMany(a => a.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    /// <summary>
    /// Adds an EPG URL to the url-tvg header attribute so players load the guide automatically.
    /// </summary>
    public void AddEpgUrl(string url)
    {
        var urls = HeaderAttributes.GetValue(UrlTvgAttribute)?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? [];

        if (!urls.Contains(url, StringComparer.OrdinalIgnoreCase))
        {
            urls.Add(url);
            HeaderAttributes.SetValue(UrlTvgAttribute, string.Join(",", urls));
        }
    }

    public static bool IsEpgAttribute(string key) =>
        string.Equals(key, UrlTvgAttribute, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(key, "x-tvg-url", StringComparison.OrdinalIgnoreCase);
}

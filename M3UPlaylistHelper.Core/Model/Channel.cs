namespace M3UPlaylistHelper.Model;

public class Channel
{
    public const string TvgIdAttribute = "tvg-id";
    public const string TvgNameAttribute = "tvg-name";
    public const string TvgLogoAttribute = "tvg-logo";
    public const string GroupTitleAttribute = "group-title";

    /// <summary>
    /// The category of the channel. Based on the group-title attribute of the channel.
    /// </summary>
    public Category Category { get; set; }

    /// <summary>
    /// The name of the channel (the text after the comma in the #EXTINF line).
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL of the channel.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The duration field of the #EXTINF line. -1 for live streams.
    /// </summary>
    public string Duration { get; set; } = "-1";

    /// <summary>
    /// All attributes of the #EXTINF line except group-title (which comes from <see cref="Category"/>), in their original order.
    /// Unknown attributes (tvg-chno, catchup, tvg-shift, ...) are preserved so they are written back when saving.
    /// </summary>
    public List<KeyValuePair<string, string>> Attributes { get; set; } = [];

    /// <summary>
    /// Other directive lines that belong to this entry, such as #EXTVLCOPT or #KODIPROP. Written back as-is.
    /// </summary>
    public List<string> ExtraLines { get; set; } = [];

    public bool IsIncluded { get; set; }

    /// <summary>
    /// The tvg-id attribute of the channel.
    /// </summary>
    public string? TvgId => GetAttribute(TvgIdAttribute);

    /// <summary>
    /// The tvg-name attribute of the channel.
    /// </summary>
    public string? TvgName => GetAttribute(TvgNameAttribute);

    /// <summary>
    /// The tvg-logo attribute of the channel.
    /// </summary>
    public string? LogoUrl => GetAttribute(TvgLogoAttribute);

    /// <summary>
    /// The title of the category this channel belongs to, for display.
    /// </summary>
    public string CategoryTitle => Category.Title;

    public Channel(Category category, string name, string url)
    {
        Category = category;
        Name = name;
        Url = url;
        IsIncluded = true;
    }

    public string? GetAttribute(string name)
    {
        foreach (var attribute in Attributes)
        {
            if (string.Equals(attribute.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrWhiteSpace(attribute.Value) ? null : attribute.Value;
            }
        }

        return null;
    }
}

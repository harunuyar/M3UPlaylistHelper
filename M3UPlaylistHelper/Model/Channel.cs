namespace M3UPlaylistHelper.Model;

public class Channel
{
    /// <summary>
    /// The category of the channel. Based on the group-title attribute of the channel.
    /// </summary>
    public Category Category { get; set; }

    /// <summary>
    /// The name of the channel.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL of the channel.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// The tvg-logo attribute of the channel.
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// The tvg-id attribute of the channel.
    /// </summary>
    public string? TvgId { get; set; }

    /// <summary>
    /// The tvg-name attribute of the channel.
    /// </summary>
    public string? TvgName { get; set; }

    public bool IsIncluded { get; set; }

    public Channel(Category category, string name, string url, string? logoUrl, string? tvgId, string? tvgName)
    {
        Category = category;
        Name = name;
        Url = url;
        LogoUrl = logoUrl;
        TvgId = tvgId;
        TvgName = tvgName;
        IsIncluded = true;
    }
}
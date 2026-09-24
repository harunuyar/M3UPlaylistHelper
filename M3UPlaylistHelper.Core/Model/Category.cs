namespace M3UPlaylistHelper.Model;

public class Category
{
    /// <summary>
    /// The group-title attribute of the channels. Renaming a category renames the group of all its channels.
    /// </summary>
    public string Title { get; set; }

    public bool IsIncluded { get; set; }

    /// <summary>
    /// The channels in the category.
    /// </summary>
    public List<Channel> Channels { get; set; }

    /// <summary>
    /// Number of channels in this category that are marked as included.
    /// </summary>
    public int IncludedChannelCount => Channels.Count(c => c.IsIncluded);

    /// <summary>
    /// "included / total" channel count, for display.
    /// </summary>
    public string ChannelCountText => $"{IncludedChannelCount} / {Channels.Count}";

    public Category(string title)
    {
        Title = title;
        Channels = [];
        IsIncluded = true;
    }
}

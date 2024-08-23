namespace M3UPlaylistHelper.Model;

public class Category
{
    /// <summary>
    /// The group-title attribute of the channel.
    /// </summary>
    public string Title { get; set; }

    public bool IsIncluded { get; set; }

    /// <summary>
    /// The channels in the category.
    /// </summary>
    public List<Channel> Channels { get; set; }

    public Category(string title)
    {
        Title = title;
        Channels = [];
        IsIncluded = true;
    }
}

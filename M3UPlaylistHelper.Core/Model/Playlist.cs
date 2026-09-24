namespace M3UPlaylistHelper.Model;

public class Playlist
{
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
}

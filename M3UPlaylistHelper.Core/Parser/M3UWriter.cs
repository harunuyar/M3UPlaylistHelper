namespace M3UPlaylistHelper.Parser;

using M3UPlaylistHelper.Model;
using System.Text;

public static class M3UWriter
{
    private static readonly UTF8Encoding Utf8WithoutBom = new(false);

    /// <summary>
    /// Serializes the included categories and channels of the playlist.
    /// </summary>
    public static string Write(Playlist playlist)
    {
        var sb = new StringBuilder();
        sb.Append("#EXTM3U");
        AppendAttributes(sb, playlist.HeaderAttributes);
        sb.Append('\n');

        foreach (var category in playlist.Categories)
        {
            if (!category.IsIncluded)
            {
                continue;
            }

            foreach (var channel in category.Channels)
            {
                if (!channel.IsIncluded)
                {
                    continue;
                }

                sb.Append("#EXTINF:").Append(channel.Duration);
                AppendAttributes(sb, channel.Attributes);
                AppendAttribute(sb, Channel.GroupTitleAttribute, category.Title);
                sb.Append(',').Append(SingleLine(channel.Name)).Append('\n');

                foreach (var extraLine in channel.ExtraLines)
                {
                    sb.Append(extraLine).Append('\n');
                }

                sb.Append(channel.Url).Append('\n');
            }
        }

        return sb.ToString();
    }

    public static void WriteFile(Playlist playlist, string filename)
    {
        // Write to a temporary file first so a failure never leaves a half-written playlist behind
        var tempFile = filename + ".tmp";
        File.WriteAllText(tempFile, Write(playlist), Utf8WithoutBom);
        File.Move(tempFile, filename, overwrite: true);
    }

    public static async Task WriteFileAsync(Playlist playlist, string filename, CancellationToken cancellationToken)
    {
        var tempFile = filename + ".tmp";
        await File.WriteAllTextAsync(tempFile, Write(playlist), Utf8WithoutBom, cancellationToken);
        File.Move(tempFile, filename, overwrite: true);
    }

    private static void AppendAttributes(StringBuilder sb, IEnumerable<KeyValuePair<string, string>> attributes)
    {
        foreach (var attribute in attributes)
        {
            AppendAttribute(sb, attribute.Key, attribute.Value);
        }
    }

    private static void AppendAttribute(StringBuilder sb, string key, string value)
    {
        // M3U has no escaping for quotes inside attribute values
        sb.Append(' ').Append(key).Append("=\"").Append(SingleLine(value).Replace('"', '\'')).Append('"');
    }

    private static string SingleLine(string value) => value.Replace("\r", " ").Replace("\n", " ");
}

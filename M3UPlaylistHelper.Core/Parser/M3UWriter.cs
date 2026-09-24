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
        using var writer = new StringWriter();
        Write(playlist, writer);
        return writer.ToString();
    }

    /// <summary>
    /// Writes the included categories and channels. Streaming to the writer keeps memory flat for huge playlists.
    /// </summary>
    public static void Write(Playlist playlist, TextWriter writer)
    {
        writer.Write("#EXTM3U");
        WriteAttributes(writer, playlist.HeaderAttributes);
        writer.Write('\n');

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

                writer.Write("#EXTINF:");
                writer.Write(channel.Duration);
                WriteAttributes(writer, channel.Attributes);
                WriteAttribute(writer, Channel.GroupTitleAttribute, category.Title);
                writer.Write(',');
                writer.Write(Sanitize(channel.Name, isAttributeValue: false));
                writer.Write('\n');

                foreach (var extraLine in channel.ExtraLines)
                {
                    writer.Write(extraLine);
                    writer.Write('\n');
                }

                writer.Write(channel.Url);
                writer.Write('\n');
            }
        }
    }

    public static void WriteFile(Playlist playlist, string filename)
    {
        // Write to a temporary file first so a failure never leaves a half-written playlist behind
        var tempFile = filename + ".tmp";

        using (var writer = new StreamWriter(tempFile, append: false, Utf8WithoutBom, bufferSize: 1 << 16))
        {
            Write(playlist, writer);
        }

        File.Move(tempFile, filename, overwrite: true);
    }

    public static Task WriteFileAsync(Playlist playlist, string filename, CancellationToken cancellationToken) =>
        Task.Run(() => WriteFile(playlist, filename), cancellationToken);

    private static void WriteAttributes(TextWriter writer, List<KeyValuePair<string, string>> attributes)
    {
        foreach (var attribute in attributes)
        {
            WriteAttribute(writer, attribute.Key, attribute.Value);
        }
    }

    private static void WriteAttribute(TextWriter writer, string key, string value)
    {
        writer.Write(' ');
        writer.Write(key);
        writer.Write("=\"");
        writer.Write(Sanitize(value, isAttributeValue: true));
        writer.Write('"');
    }

    private static readonly char[] lineBreaks = ['\r', '\n'];
    private static readonly char[] lineBreaksAndQuote = ['\r', '\n', '"'];

    /// <summary>
    /// Keeps values on one line; M3U has no escaping for quotes inside attribute values, so those become apostrophes.
    /// Returns the same string (no allocation) in the common case where there is nothing to fix.
    /// </summary>
    private static string Sanitize(string value, bool isAttributeValue)
    {
        if (value.IndexOfAny(isAttributeValue ? lineBreaksAndQuote : lineBreaks) < 0)
        {
            return value;
        }

        var result = value.Replace('\r', ' ').Replace('\n', ' ');
        return isAttributeValue ? result.Replace('"', '\'') : result;
    }
}

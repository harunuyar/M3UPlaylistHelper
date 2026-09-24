namespace M3UPlaylistHelper.Parser;

using M3UPlaylistHelper.Model;
using System.Text;

public static class M3UParser
{
    public const string DefaultCategory = "Unknown";

    private const string HeaderTag = "#EXTM3U";
    private const string ExtInfTag = "#EXTINF:";
    private const string ExtGrpTag = "#EXTGRP:";

    private static readonly Encoding strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private static readonly Encoding windows1252 = CreateWindows1252();

    private static Encoding CreateWindows1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    }

    public static async Task<Playlist> ParseUrlAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await Http.Client.GetAsync(url, cancellationToken);

        Http.EnsureSuccess(response);

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var charset = response.Content.Headers.ContentType?.CharSet;
        return Parse(Decode(bytes, charset));
    }

    public static async Task<Playlist> ParseFileAsync(string filename, CancellationToken cancellationToken)
    {
        var bytes = await File.ReadAllBytesAsync(filename, cancellationToken);
        return Parse(Decode(bytes, null));
    }

    /// <summary>
    /// Decodes playlist bytes. M3U8 is UTF-8, but plenty of older .m3u files are saved in the Windows ANSI code page;
    /// reading those as UTF-8 would replace accented characters with '?' and saving would make that permanent.
    /// </summary>
    public static string Decode(byte[] bytes, string? charset)
    {
        if (!string.IsNullOrWhiteSpace(charset))
        {
            try
            {
                return StripBom(Encoding.GetEncoding(charset.Trim('"')).GetString(bytes));
            }
            catch (ArgumentException)
            {
                // Unknown charset, detect it below
            }
        }

        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
        {
            return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
        }

        if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
        {
            return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
        }

        try
        {
            return StripBom(strictUtf8.GetString(bytes));
        }
        catch (DecoderFallbackException)
        {
            return windows1252.GetString(bytes);
        }
    }

    private static string StripBom(string text) => text.Length > 0 && text[0] == '\uFEFF' ? text[1..] : text;

    public static Playlist Parse(string content)
    {
        var playlist = new Playlist();
        Dictionary<string, Category> categoryDict = [];

        // State of the entry being read. An entry ends with its URL line.
        string? pendingExtInf = null;
        string? pendingGroup = null;
        List<string>? pendingExtraLines = null;
        bool seenContent = false;
        int lineNumber = 0;

        // Work on spans: playlists can have hundreds of thousands of lines, and only the parts that are kept become strings
        foreach (var rawLine in content.AsSpan().EnumerateLines())
        {
            lineNumber++;
            var line = rawLine.Trim().TrimStart('\uFEFF');

            if (line.IsEmpty)
            {
                continue;
            }

            if (line.StartsWith(HeaderTag, StringComparison.OrdinalIgnoreCase))
            {
                // Only the first header counts, later ones come from concatenated playlists
                if (!seenContent)
                {
                    playlist.HeaderAttributes = ParseAttributes(line.ToString(), HeaderTag.Length, out _);
                }

                seenContent = true;
                continue;
            }

            seenContent = true;

            if (line.StartsWith(ExtInfTag, StringComparison.OrdinalIgnoreCase))
            {
                if (pendingExtInf != null)
                {
                    // The previous entry had no URL, don't let its group and options leak into this one
                    Console.Error.WriteLine($"Entry without URL before line {lineNumber}: {pendingExtInf}");
                    pendingGroup = null;
                    pendingExtraLines = null;
                }

                pendingExtInf = line.ToString();
            }
            else if (line.StartsWith(ExtGrpTag, StringComparison.OrdinalIgnoreCase))
            {
                pendingGroup = line[ExtGrpTag.Length..].Trim().ToString();
            }
            else if (line[0] == '#')
            {
                // #EXTVLCOPT, #KODIPROP, ... belong to the entry. Plain comments and HLS tags (#EXT-X-...) are dropped.
                bool isEntryDirective = line.StartsWith("#EXT", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("#EXT-X-", StringComparison.OrdinalIgnoreCase);
                if (isEntryDirective || line.StartsWith("#KODIPROP", StringComparison.OrdinalIgnoreCase))
                {
                    (pendingExtraLines ??= []).Add(line.ToString());
                }
            }
            else
            {
                var channel = CreateChannel(pendingExtInf, pendingGroup, line.ToString(), pendingExtraLines ?? [], playlist, categoryDict);
                channel.Category.Channels.Add(channel);

                pendingExtInf = null;
                pendingGroup = null;
                pendingExtraLines = null;
            }
        }

        return playlist;
    }

    private static Channel CreateChannel(
        string? extInf,
        string? group,
        string url,
        List<string> extraLines,
        Playlist playlist,
        Dictionary<string, Category> categoryDict)
    {
        string duration = "-1";
        string name = url;
        List<KeyValuePair<string, string>> attributes = [];

        if (extInf != null)
        {
            ParseExtInf(extInf, out duration, out attributes, out var parsedName);

            if (!string.IsNullOrWhiteSpace(parsedName))
            {
                name = parsedName;
            }
        }

        // group-title is written from the category, so remove every copy of it (some lines have it twice)
        string? groupTitle = null;
        for (int i = attributes.Count - 1; i >= 0; i--)
        {
            if (string.Equals(attributes[i].Key, Channel.GroupTitleAttribute, StringComparison.OrdinalIgnoreCase))
            {
                var value = attributes[i].Value.Trim();
                if (value.Length > 0)
                {
                    // Iterating backwards, so the first non-empty one wins
                    groupTitle = value;
                }

                attributes.RemoveAt(i);
            }
        }

        if (groupTitle != null)
        {
            group = groupTitle;
        }

        if (string.IsNullOrWhiteSpace(group))
        {
            group = DefaultCategory;
        }

        if (!categoryDict.TryGetValue(group, out var category))
        {
            category = new Category(group);
            categoryDict[group] = category;
            playlist.Categories.Add(category);
        }

        return new Channel(category, name, url)
        {
            Duration = duration,
            Attributes = attributes,
            ExtraLines = extraLines,
        };
    }

    /// <summary>
    /// Parses a line in the form <c>#EXTINF:-1 key="value" key2="value, with comma",Channel name</c>.
    /// </summary>
    public static void ParseExtInf(string line, out string duration, out List<KeyValuePair<string, string>> attributes, out string name)
    {
        int pos = line.StartsWith(ExtInfTag, StringComparison.OrdinalIgnoreCase) ? ExtInfTag.Length : 0;

        int durationStart = pos;
        while (pos < line.Length && !char.IsWhiteSpace(line[pos]) && line[pos] != ',')
        {
            pos++;
        }

        duration = line[durationStart..pos];
        if (duration.Length == 0)
        {
            duration = "-1";
        }

        attributes = ParseAttributes(line, pos, out pos);
        name = pos < line.Length ? line[(pos + 1)..].Trim() : string.Empty;
    }

    /// <summary>
    /// Parses <c>key="value"</c> pairs starting at <paramref name="start"/>. Stops at the first comma outside of quotes
    /// (or at the end of the line); <paramref name="end"/> is the index of that comma.
    /// </summary>
    private static List<KeyValuePair<string, string>> ParseAttributes(string line, int start, out int end)
    {
        List<KeyValuePair<string, string>> attributes = [];
        int pos = start;

        while (pos < line.Length)
        {
            while (pos < line.Length && char.IsWhiteSpace(line[pos]))
            {
                pos++;
            }

            if (pos >= line.Length || line[pos] == ',')
            {
                break;
            }

            int keyStart = pos;
            while (pos < line.Length && line[pos] != '=' && line[pos] != ',' && !char.IsWhiteSpace(line[pos]))
            {
                pos++;
            }

            var key = line[keyStart..pos];

            if (pos >= line.Length || line[pos] != '=')
            {
                // Key without value, ignore it
                continue;
            }

            pos++; // skip '='

            string value;
            if (pos < line.Length && (line[pos] == '"' || line[pos] == '\''))
            {
                char quote = line[pos++];
                int valueStart = pos;
                int closing = line.IndexOf(quote, pos);

                if (closing < 0)
                {
                    value = line[valueStart..];
                    pos = line.Length;
                }
                else
                {
                    value = line[valueStart..closing];
                    pos = closing + 1;
                }
            }
            else
            {
                int valueStart = pos;
                while (pos < line.Length && line[pos] != ',' && !char.IsWhiteSpace(line[pos]))
                {
                    pos++;
                }

                value = line[valueStart..pos];
            }

            if (key.Length > 0)
            {
                attributes.Add(new KeyValuePair<string, string>(key, value));
            }
        }

        end = pos;
        return attributes;
    }
}

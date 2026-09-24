namespace M3UPlaylistHelper.Parser;

using M3UPlaylistHelper.Model;
using System.Net;
using System.Text;

public static class M3UParser
{
    public const string DefaultCategory = "Unknown";

    private const string HeaderTag = "#EXTM3U";
    private const string ExtInfTag = "#EXTINF:";
    private const string ExtGrpTag = "#EXTGRP:";

    private static readonly HttpClient httpClient = CreateHttpClient();

    private static readonly Encoding strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private static readonly Encoding windows1252 = CreateWindows1252();

    private static Encoding CreateWindows1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    }

    private static HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(2),
        };

        // Some providers block requests without a user agent
        client.DefaultRequestHeaders.UserAgent.ParseAdd("M3UPlaylistHelper");
        return client;
    }

    public static async Task<Playlist> ParseUrlAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"The server returned {(int)response.StatusCode} ({response.ReasonPhrase}).", null, response.StatusCode);
        }

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
        var lines = content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        return Parse(lines);
    }

    public static Playlist Parse(IReadOnlyList<string> lines)
    {
        var playlist = new Playlist();
        Dictionary<string, Category> categoryDict = [];

        // State of the entry being read. An entry ends with its URL line.
        string? pendingExtInf = null;
        string? pendingGroup = null;
        List<string> pendingExtraLines = [];
        bool seenContent = false;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i].Trim().TrimStart('﻿');

            if (line.Length == 0)
            {
                continue;
            }

            if (line.StartsWith(HeaderTag, StringComparison.OrdinalIgnoreCase))
            {
                // Only the first header counts, later ones come from concatenated playlists
                if (!seenContent)
                {
                    playlist.HeaderAttributes = ParseAttributes(line, HeaderTag.Length, out _);
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
                    Console.Error.WriteLine($"Entry without URL before line {i + 1}: {pendingExtInf}");
                    pendingGroup = null;
                    pendingExtraLines = [];
                }

                pendingExtInf = line;
            }
            else if (line.StartsWith(ExtGrpTag, StringComparison.OrdinalIgnoreCase))
            {
                pendingGroup = line[ExtGrpTag.Length..].Trim();
            }
            else if (line.StartsWith('#'))
            {
                // #EXTVLCOPT, #KODIPROP, ... belong to the entry. Plain comments and HLS tags (#EXT-X-...) are dropped.
                bool isEntryDirective = line.StartsWith("#EXT", StringComparison.OrdinalIgnoreCase) && !line.StartsWith("#EXT-X-", StringComparison.OrdinalIgnoreCase);
                if (isEntryDirective || line.StartsWith("#KODIPROP", StringComparison.OrdinalIgnoreCase))
                {
                    pendingExtraLines.Add(line);
                }
            }
            else
            {
                var channel = CreateChannel(pendingExtInf, pendingGroup, line, pendingExtraLines, playlist, categoryDict);
                channel.Category.Channels.Add(channel);

                pendingExtInf = null;
                pendingGroup = null;
                pendingExtraLines = [];
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
        static bool IsGroupTitle(KeyValuePair<string, string> a) => string.Equals(a.Key, Channel.GroupTitleAttribute, StringComparison.OrdinalIgnoreCase);
        var groupTitle = attributes.Where(IsGroupTitle).Select(a => a.Value.Trim()).FirstOrDefault(v => v.Length > 0);
        if (groupTitle != null)
        {
            group = groupTitle;
        }

        attributes.RemoveAll(IsGroupTitle);

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

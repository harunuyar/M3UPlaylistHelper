namespace M3UPlaylistHelper.Epg;

using M3UPlaylistHelper.Model;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

/// <summary>
/// The channel list of an XMLTV electronic program guide. Only the &lt;channel&gt; elements are read: they come before the
/// programmes, so reading stops at the first &lt;programme&gt; and even huge guides load in a moment.
/// </summary>
public class XmlTvGuide
{
    private static readonly Regex countryPrefix = new(@"^[\[(|]?\s*[A-Za-z]{2,3}\s*[\])|:-]\s*(?=\S)", RegexOptions.Compiled);

    private readonly HashSet<string> channelIds = new(StringComparer.OrdinalIgnoreCase);

    // Normalized display name -> channel ids with that name
    private readonly Dictionary<string, HashSet<string>> idsByName = [];

    public int ChannelCount => channelIds.Count;

    public bool ContainsId(string? id) => id != null && channelIds.Contains(id.Trim());

    /// <summary>
    /// Whether the channel's tvg-id is listed in the guide, i.e. players will show program information for it.
    /// </summary>
    public bool HasGuide(Channel channel) => ContainsId(channel.TvgId);

    public void AddChannel(string id, IEnumerable<string> displayNames)
    {
        id = id.Trim();
        if (id.Length == 0)
        {
            return;
        }

        channelIds.Add(id);

        foreach (var name in displayNames.Append(id))
        {
            var key = NormalizeName(name);
            if (key.Length == 0)
            {
                continue;
            }

            if (!idsByName.TryGetValue(key, out var ids))
            {
                ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                idsByName[key] = ids;
            }

            ids.Add(id);
        }
    }

    /// <summary>
    /// Adds the channels of another guide, e.g. when a playlist lists several EPG URLs.
    /// </summary>
    public void Add(XmlTvGuide other)
    {
        foreach (var (name, ids) in other.idsByName)
        {
            if (!idsByName.TryGetValue(name, out var existing))
            {
                existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                idsByName[name] = existing;
            }

            existing.UnionWith(ids);
        }

        channelIds.UnionWith(other.channelIds);
    }

    /// <summary>
    /// Finds the guide channel id for a channel name, when exactly one guide channel has that name.
    /// </summary>
    public string? FindIdByName(string? name)
    {
        var key = NormalizeName(name);
        return key.Length > 0 && idsByName.TryGetValue(key, out var ids) && ids.Count == 1 ? ids.First() : null;
    }

    /// <summary>
    /// Sets tvg-id on channels that have none (or one the guide doesn't know) when their tvg-name or name matches
    /// exactly one guide channel.
    /// </summary>
    /// <returns>The number of channels that got a tvg-id.</returns>
    public int FillMissingTvgIds(Playlist playlist)
    {
        int count = 0;

        foreach (var channel in playlist.AllChannels)
        {
            if (HasGuide(channel))
            {
                continue;
            }

            var id = FindIdByName(channel.TvgName) ?? FindIdByName(channel.Name);
            if (id != null)
            {
                channel.SetAttribute(Channel.TvgIdAttribute, id);
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Unchecks every included channel that has no guide data.
    /// </summary>
    /// <returns>The number of channels that were excluded.</returns>
    public int ExcludeChannelsWithoutGuide(Playlist playlist)
    {
        int count = 0;

        foreach (var channel in playlist.ExportedChannels)
        {
            if (!HasGuide(channel))
            {
                channel.IsIncluded = false;
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Lower case letters and digits only, without quality suffixes and country prefixes, so that "UK: BBC One HD"
    /// and "BBC One" compare equal.
    /// </summary>
    public static string NormalizeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        // Drop "UK:" / "|UK|" / "[UK]" style country prefixes
        var text = countryPrefix.Replace(name.Trim(), string.Empty);

        var words = new StringBuilder(text.Length)
            .Append(text.ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : ' ').ToArray())
            .ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        string[] qualityWords = ["hd", "fhd", "uhd", "sd", "4k", "hevc", "h265", "1080p", "720p", "backup"];
        while (words.Count > 1 && qualityWords.Contains(words[^1]))
        {
            words.RemoveAt(words.Count - 1);
        }

        return string.Concat(words);
    }

    public static async Task<XmlTvGuide> LoadAsync(Stream stream, CancellationToken cancellationToken)
    {
        // .xml.gz guides are usually served as plain files, without Content-Encoding, so look at the magic bytes
        var header = new byte[2];
        int read = await stream.ReadAtLeastAsync(header, 2, throwOnEndOfStream: false, cancellationToken);
        Stream content = new PrefixedStream(header.AsMemory(0, read).ToArray(), stream);

        if (read == 2 && header[0] == 0x1F && header[1] == 0x8B)
        {
            content = new GZipStream(content, CompressionMode.Decompress);
        }

        var settings = new XmlReaderSettings
        {
            Async = true,
            DtdProcessing = DtdProcessing.Ignore,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
        };

        var guide = new XmlTvGuide();
        using var reader = XmlReader.Create(content, settings);

        while (await reader.ReadAsync())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }

            if (reader.LocalName == "programme")
            {
                break;
            }

            if (reader.LocalName == "channel" && reader.GetAttribute("id") is string id)
            {
                guide.AddChannel(id, await ReadDisplayNamesAsync(reader));
            }
        }

        return guide;
    }

    private static async Task<List<string>> ReadDisplayNamesAsync(XmlReader reader)
    {
        List<string> names = [];
        using var channel = reader.ReadSubtree();
        await channel.ReadAsync();

        while (!channel.EOF)
        {
            // ReadElementContentAsString moves to the next node by itself, so only call ReadAsync otherwise
            if (channel.NodeType == XmlNodeType.Element && channel.LocalName == "display-name")
            {
                names.Add(await channel.ReadElementContentAsStringAsync());
            }
            else
            {
                await channel.ReadAsync();
            }
        }

        return names;
    }

    public static async Task<XmlTvGuide> LoadFileAsync(string filename, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filename);
        return await LoadAsync(stream, cancellationToken);
    }

    public static async Task<XmlTvGuide> LoadUrlAsync(string url, CancellationToken cancellationToken)
    {
        using var response = await Http.Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        Http.EnsureSuccess(response);
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await LoadAsync(stream, cancellationToken);
    }

    /// <summary>
    /// A read-only stream that returns some already consumed bytes before the rest of the inner stream.
    /// </summary>
    private sealed class PrefixedStream(byte[] prefix, Stream inner) : Stream
    {
        private int position;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));

        public override int Read(Span<byte> buffer)
        {
            if (position < prefix.Length)
            {
                int n = Math.Min(buffer.Length, prefix.Length - position);
                prefix.AsSpan(position, n).CopyTo(buffer);
                position += n;
                return n;
            }

            return inner.Read(buffer);
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (position < prefix.Length)
            {
                return Read(buffer.Span);
            }

            return await inner.ReadAsync(buffer, cancellationToken);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}

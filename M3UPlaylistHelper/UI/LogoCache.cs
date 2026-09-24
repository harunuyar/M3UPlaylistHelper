namespace M3UPlaylistHelper.UI;

using System.Collections.Concurrent;
using System.Drawing.Drawing2D;

/// <summary>
/// Downloads channel logos once and keeps small thumbnails in memory.
/// </summary>
public static class LogoCache
{
    public const int ThumbnailWidth = 64;
    public const int ThumbnailHeight = 40;

    private const int MaxParallelDownloads = 8;

    private static readonly HttpClient httpClient = CreateHttpClient();

    // A null value means the download was attempted and failed, so it is not retried
    private static readonly ConcurrentDictionary<string, Image?> logos = new();

    private static HttpClient CreateHttpClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("M3UPlaylistHelper");
        return client;
    }

    public static Image? Get(string? url) =>
        url != null && logos.TryGetValue(url, out var image) ? image : null;

    /// <summary>
    /// Downloads the logos that are not cached yet. <paramref name="onBatchLoaded"/> is called (from a background thread)
    /// every few logos so the UI can repaint progressively.
    /// </summary>
    public static async Task LoadAsync(IEnumerable<string?> urls, Action onBatchLoaded, CancellationToken cancellationToken)
    {
        var pending = urls
            .Where(url => !string.IsNullOrWhiteSpace(url) && !logos.ContainsKey(url))
            .Distinct()
            .Cast<string>()
            .ToList();

        if (pending.Count == 0)
        {
            return;
        }

        int loaded = 0;
        var options = new ParallelOptions { MaxDegreeOfParallelism = MaxParallelDownloads, CancellationToken = cancellationToken };

        try
        {
            await Parallel.ForEachAsync(pending, options, async (url, token) =>
            {
                logos[url] = await DownloadThumbnailAsync(url, token);

                if (Interlocked.Increment(ref loaded) % MaxParallelDownloads == 0)
                {
                    onBatchLoaded();
                }
            });
        }
        catch (OperationCanceledException)
        {
            return;
        }

        onBatchLoaded();
    }

    private static async Task<Image?> DownloadThumbnailAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var bytes = await httpClient.GetByteArrayAsync(url, cancellationToken);
            using var stream = new MemoryStream(bytes);
            using var original = Image.FromStream(stream);
            return CreateThumbnail(original);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception)
        {
            Console.Error.WriteLine($"Failed to load logo: {url}");
            return null;
        }
    }

    private static Bitmap CreateThumbnail(Image original)
    {
        double scale = Math.Min((double)ThumbnailWidth / original.Width, (double)ThumbnailHeight / original.Height);
        int width = Math.Max(1, (int)(original.Width * scale));
        int height = Math.Max(1, (int)(original.Height * scale));

        var thumbnail = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(thumbnail);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(original, 0, 0, width, height);
        return thumbnail;
    }
}

namespace M3UPlaylistHelper.Model;

public class Channel
{
    public const int LogoWidth = 50;

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

    public Image? Logo { get; set; }

    private bool AttemptedLogoLoad { get; set; }

    public Channel(Category category, string name, string url, string? logoUrl, string? tvgId, string? tvgName)
    {
        Category = category;
        Name = name;
        Url = url;
        LogoUrl = logoUrl;
        TvgId = tvgId;
        TvgName = tvgName;
        IsIncluded = true;
        Logo = null;
        AttemptedLogoLoad = false;
    }

    public async Task LoadLogoAsync()
    {
        if (AttemptedLogoLoad)
        {
            return;
        }

        AttemptedLogoLoad = true;

        if (LogoUrl == null)
        {
            return;
        }

        try
        {
            CancellationToken cancellationToken = new CancellationTokenSource(5000).Token;
            Logo = Image.FromStream(await new HttpClient().GetStreamAsync(LogoUrl, cancellationToken));
            Logo = new Bitmap(Logo, new Size(LogoWidth, Logo.Height * LogoWidth / Logo.Width));
        }
        catch (Exception)
        {
            Console.Error.WriteLine($"Failed to load logo for channel: {Name}, Logo url: {LogoUrl}");
        }
    }
}
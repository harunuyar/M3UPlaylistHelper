namespace M3UPlaylistHelper;

using System.Text.Json;

/// <summary>
/// User preferences, stored in %AppData%\M3UPlaylistHelper\settings.json.
/// </summary>
public class AppSettings
{
    private const int MaxRecentItems = 10;

    private static readonly string settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "M3UPlaylistHelper",
        "settings.json");

    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Recently opened files and URLs, most recent first.
    /// </summary>
    public List<string> RecentItems { get; set; } = [];

    public bool DownloadLogos { get; set; } = true;

    public int? WindowX { get; set; }
    public int? WindowY { get; set; }
    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }
    public bool WindowMaximized { get; set; }
    public int? SplitterDistance { get; set; }

    public IEnumerable<string> RecentUrls => RecentItems.Where(IsUrl);

    public static bool IsUrl(string item) =>
        Uri.TryCreate(item, UriKind.Absolute, out var uri) && !uri.IsFile;

    public void AddRecentItem(string item)
    {
        RecentItems.RemoveAll(i => string.Equals(i, item, StringComparison.OrdinalIgnoreCase));
        RecentItems.Insert(0, item);

        if (RecentItems.Count > MaxRecentItems)
        {
            RecentItems.RemoveRange(MaxRecentItems, RecentItems.Count - MaxRecentItems);
        }
    }

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settingsPath), jsonOptions) ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load settings: {ex.Message}");
        }

        return new AppSettings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(this, jsonOptions));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}

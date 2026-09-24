namespace M3UPlaylistHelper;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

/// <summary>
/// User preferences, stored in %AppData%\M3UPlaylistHelper\settings.json, or next to the executable in portable mode.
/// </summary>
public class AppSettings
{
    private const int MaxRecentItems = 10;

    /// <summary>
    /// When a file with this name is next to the executable, settings are kept there instead of in %AppData%,
    /// so the app can run from a USB stick without leaving anything behind.
    /// </summary>
    public const string PortableMarkerFile = "portable.txt";

    private static readonly string settingsPath = GetSettingsPath();

    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    public static bool IsPortable { get; private set; }

    /// <summary>
    /// Recently opened files and URLs, most recent first.
    /// </summary>
    public List<string> RecentItems { get; set; } = [];

    /// <summary>
    /// The last URL typed in the Open URL dialog, even if loading it failed, so a typo can be fixed.
    /// </summary>
    public string? LastUrl { get; set; }

    public string? LastEpgUrl { get; set; }

    public bool DownloadLogos { get; set; } = true;

    /// <summary>
    /// "System", "Light" or "Dark".
    /// </summary>
    public string Theme { get; set; } = "System";

    public string? XtreamServer { get; set; }
    public string? XtreamUsername { get; set; }
    public string? XtreamOutput { get; set; }

    /// <summary>
    /// Encrypted with Windows DPAPI, only the current Windows user can decrypt it.
    /// </summary>
    public string? XtreamPasswordProtected { get; set; }

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

    public string? GetXtreamPassword()
    {
        if (string.IsNullOrEmpty(XtreamPasswordProtected))
        {
            return null;
        }

        try
        {
            var bytes = ProtectedData.Unprotect(Convert.FromBase64String(XtreamPasswordProtected), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch (Exception)
        {
            // Settings copied from another user or machine
            return null;
        }
    }

    public void SetXtreamPassword(string? password)
    {
        XtreamPasswordProtected = string.IsNullOrEmpty(password)
            ? null
            : Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(password), null, DataProtectionScope.CurrentUser));
    }

    private static string GetSettingsPath()
    {
        if (File.Exists(Path.Combine(AppContext.BaseDirectory, PortableMarkerFile)))
        {
            IsPortable = true;
            return Path.Combine(AppContext.BaseDirectory, "settings.json");
        }

        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "M3UPlaylistHelper", "settings.json");
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

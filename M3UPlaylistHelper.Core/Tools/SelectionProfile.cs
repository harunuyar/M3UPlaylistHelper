namespace M3UPlaylistHelper.Tools;

using M3UPlaylistHelper.Model;
using System.Text.Json;

/// <summary>
/// A saved set of include/exclude choices. Providers regenerate their playlists all the time, so instead of
/// picking the same categories and channels again after every update, a profile can be saved once and applied
/// to the fresh playlist.
/// </summary>
public class SelectionProfile
{
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Categories that were included when the profile was captured.
    /// </summary>
    public List<string> IncludedCategories { get; set; } = [];

    /// <summary>
    /// Categories that were excluded when the profile was captured.
    /// </summary>
    public List<string> ExcludedCategories { get; set; } = [];

    /// <summary>
    /// Excluded channel names, keyed by category title.
    /// </summary>
    public Dictionary<string, List<string>> ExcludedChannels { get; set; } = [];

    /// <summary>
    /// Whether categories that are not known to this profile (new categories added by the provider) are included.
    /// </summary>
    public bool IncludeNewCategories { get; set; } = true;

    public static SelectionProfile Capture(Playlist playlist, bool includeNewCategories)
    {
        var profile = new SelectionProfile { IncludeNewCategories = includeNewCategories };

        foreach (var category in playlist.Categories)
        {
            (category.IsIncluded ? profile.IncludedCategories : profile.ExcludedCategories).Add(category.Title);

            var excludedChannels = category.Channels.Where(c => !c.IsIncluded).Select(c => c.Name).Distinct().ToList();
            if (excludedChannels.Count > 0)
            {
                profile.ExcludedChannels[category.Title] = excludedChannels;
            }
        }

        return profile;
    }

    public void Apply(Playlist playlist)
    {
        var included = new HashSet<string>(IncludedCategories, StringComparer.OrdinalIgnoreCase);
        var excluded = new HashSet<string>(ExcludedCategories, StringComparer.OrdinalIgnoreCase);
        var excludedChannels = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var (category, names) in ExcludedChannels)
        {
            excludedChannels[category] = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
        }

        foreach (var category in playlist.Categories)
        {
            category.IsIncluded = excluded.Contains(category.Title)
                ? false
                : included.Contains(category.Title) || IncludeNewCategories;

            excludedChannels.TryGetValue(category.Title, out var excludedNames);

            foreach (var channel in category.Channels)
            {
                channel.IsIncluded = excludedNames == null || !excludedNames.Contains(channel.Name);
            }
        }
    }

    public async Task SaveAsync(string filename, CancellationToken cancellationToken)
    {
        await using var stream = File.Create(filename);
        await JsonSerializer.SerializeAsync(stream, this, jsonOptions, cancellationToken);
    }

    public static async Task<SelectionProfile> LoadAsync(string filename, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filename);
        return await JsonSerializer.DeserializeAsync<SelectionProfile>(stream, jsonOptions, cancellationToken)
            ?? throw new InvalidDataException("The selection profile is empty.");
    }
}

namespace M3UPlaylistHelper.Tools;

using M3UPlaylistHelper.Model;

public static class PlaylistTools
{
    /// <summary>
    /// Excludes every included channel whose URL was already seen in an earlier included channel.
    /// Nothing is deleted, the duplicates are only unchecked.
    /// </summary>
    /// <returns>The number of channels that were excluded.</returns>
    public static int ExcludeDuplicateChannels(Playlist playlist)
    {
        HashSet<string> seenUrls = new(StringComparer.OrdinalIgnoreCase);
        int count = 0;

        foreach (var channel in playlist.ExportedChannels)
        {
            if (!seenUrls.Add(channel.Url.Trim()))
            {
                channel.IsIncluded = false;
                count++;
            }
        }

        return count;
    }

    public static void SortCategories(Playlist playlist)
    {
        playlist.Categories.Sort((a, b) => string.Compare(a.Title, b.Title, StringComparison.CurrentCultureIgnoreCase));
    }

    /// <summary>
    /// Moves a category by <paramref name="offset"/> positions, clamped to the bounds of the list.
    /// </summary>
    /// <returns>The new index of the category.</returns>
    public static int MoveCategory(Playlist playlist, Category category, int offset)
    {
        int index = playlist.Categories.IndexOf(category);
        if (index < 0)
        {
            return index;
        }

        int newIndex = Math.Clamp(index + offset, 0, playlist.Categories.Count - 1);
        playlist.Categories.RemoveAt(index);
        playlist.Categories.Insert(newIndex, category);
        return newIndex;
    }

    public static bool Matches(string? text, string filter) =>
        text != null && text.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
}

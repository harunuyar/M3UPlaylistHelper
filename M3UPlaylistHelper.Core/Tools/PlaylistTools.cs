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

    /// <summary>
    /// Moves <paramref name="category"/> in front of <paramref name="before"/>, or to the end if it is null.
    /// </summary>
    /// <returns>false if the order did not change.</returns>
    public static bool MoveCategoryBefore(Playlist playlist, Category category, Category? before)
    {
        int current = playlist.Categories.IndexOf(category);
        int next = current + 1;
        bool alreadyThere = before == null ? next == playlist.Categories.Count : next < playlist.Categories.Count && playlist.Categories[next] == before;

        if (current < 0 || category == before || alreadyThere)
        {
            return false;
        }

        playlist.Categories.RemoveAt(current);
        int index = before == null ? -1 : playlist.Categories.IndexOf(before);
        playlist.Categories.Insert(index < 0 ? playlist.Categories.Count : index, category);
        return true;
    }

    /// <summary>
    /// Moves channels into <paramref name="target"/>, in front of <paramref name="before"/> (or at the end if it is null),
    /// keeping their relative order. Works both for reordering inside a category and for moving between categories.
    /// Categories left without channels are removed.
    /// </summary>
    /// <returns>false if nothing was moved.</returns>
    public static bool MoveChannels(Playlist playlist, IReadOnlyCollection<Channel> channels, Category target, Channel? before)
    {
        // Sets and RemoveAll keep this linear, moving thousands of channels out of a huge category must not be quadratic
        var moving = new HashSet<Channel>(channels);

        if (moving.Count == 0 || (before != null && moving.Contains(before)))
        {
            return false;
        }

        var ordered = channels.Distinct().ToList();

        if (IsAlreadyInPlace(ordered, target, before))
        {
            return false;
        }

        var sourceCategories = moving.Select(c => c.Category).Distinct().ToList();

        foreach (var category in sourceCategories)
        {
            category.Channels.RemoveAll(moving.Contains);
        }

        foreach (var channel in ordered)
        {
            channel.Category = target;
        }

        int index = before == null ? -1 : target.Channels.IndexOf(before);
        target.Channels.InsertRange(index < 0 ? target.Channels.Count : index, ordered);

        if (!playlist.Categories.Contains(target))
        {
            playlist.Categories.Add(target);
        }

        var emptied = sourceCategories.Where(c => c != target && c.Channels.Count == 0).ToHashSet();
        if (emptied.Count > 0)
        {
            playlist.Categories.RemoveAll(emptied.Contains);
        }

        return true;
    }

    /// <summary>
    /// Whether the channels already sit, in this order, right in front of <paramref name="before"/> (or at the end).
    /// </summary>
    private static bool IsAlreadyInPlace(List<Channel> channels, Category target, Channel? before)
    {
        if (channels.Any(c => c.Category != target))
        {
            return false;
        }

        int start = target.Channels.IndexOf(channels[0]);
        int end = start + channels.Count;

        if (start < 0 || end > target.Channels.Count)
        {
            return false;
        }

        for (int i = 0; i < channels.Count; i++)
        {
            if (target.Channels[start + i] != channels[i])
            {
                return false;
            }
        }

        return before == null ? end == target.Channels.Count : end < target.Channels.Count && target.Channels[end] == before;
    }

    /// <summary>
    /// Returns the category with the given title (case-insensitive), or null.
    /// </summary>
    public static Category? FindCategory(Playlist playlist, string title) =>
        playlist.Categories.FirstOrDefault(c => string.Equals(c.Title, title, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Appends the channels of <paramref name="source"/> to <paramref name="target"/>. Categories with the same title are
    /// merged, EPG URLs from both headers are kept.
    /// </summary>
    /// <returns>The number of channels added.</returns>
    public static int Merge(Playlist target, Playlist source)
    {
        foreach (var attribute in source.HeaderAttributes)
        {
            if (!Playlist.IsEpgAttribute(attribute.Key) && target.HeaderAttributes.GetValue(attribute.Key) == null)
            {
                target.HeaderAttributes.SetValue(attribute.Key, attribute.Value);
            }
        }

        foreach (var url in source.EpgUrls)
        {
            if (!target.EpgUrls.Contains(url, StringComparer.OrdinalIgnoreCase))
            {
                target.AddEpgUrl(url);
            }
        }

        int count = 0;
        var categoriesByTitle = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

        foreach (var category in target.Categories)
        {
            categoriesByTitle.TryAdd(category.Title, category);
        }

        foreach (var sourceCategory in source.Categories)
        {
            if (!categoriesByTitle.TryGetValue(sourceCategory.Title, out var category))
            {
                category = new Category(sourceCategory.Title) { IsIncluded = sourceCategory.IsIncluded };
                target.Categories.Add(category);
                categoriesByTitle[category.Title] = category;
            }

            foreach (var channel in sourceCategory.Channels)
            {
                channel.Category = category;
                category.Channels.Add(channel);
                count++;
            }
        }

        return count;
    }

    public static bool Matches(string? text, string filter) =>
        text != null && text.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
}

namespace M3UPlaylistHelper.Model;

public static class AttributeListExtensions
{
    /// <summary>
    /// Returns the value of the first attribute named <paramref name="name"/>, or null if it is missing or blank.
    /// </summary>
    public static string? GetValue(this List<KeyValuePair<string, string>> attributes, string name)
    {
        foreach (var attribute in attributes)
        {
            if (string.Equals(attribute.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                return string.IsNullOrWhiteSpace(attribute.Value) ? null : attribute.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Replaces the value of the attribute named <paramref name="name"/>, keeping its position, or appends it.
    /// </summary>
    public static void SetValue(this List<KeyValuePair<string, string>> attributes, string name, string value)
    {
        int index = attributes.FindIndex(a => string.Equals(a.Key, name, StringComparison.OrdinalIgnoreCase));

        if (index >= 0)
        {
            attributes[index] = new KeyValuePair<string, string>(attributes[index].Key, value);
        }
        else
        {
            attributes.Add(new KeyValuePair<string, string>(name, value));
        }
    }
}

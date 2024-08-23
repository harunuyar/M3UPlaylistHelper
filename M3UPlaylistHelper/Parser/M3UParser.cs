namespace M3UPlaylistHelper.Parser;

using M3UPlaylistHelper.Model;
using System.Text;
using System.Text.RegularExpressions;

public static class M3UParser
{
    private const string DefaultCategory = "Unknown";

    private static string? GetAttributeValue(string line, string attributeName)
    {
        var match = Regex.Match(line, $"{attributeName}=\"(.*?)\"");
        return match.Success ? match.Groups[1].Value : null;
    }

    public static async Task<List<Category>> ParseAsync(string filename, CancellationToken cancellationToken)
    {
        Dictionary<string, Category> categoryDict = [];
        List<Category> categories = [];

        var lines = await File.ReadAllLinesAsync(filename, cancellationToken);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line.StartsWith("#EXTINF:"))
            {
                var tvgId = GetAttributeValue(line, "tvg-id");
                var tvgName = GetAttributeValue(line, "tvg-name");
                var tvgLogo = GetAttributeValue(line, "tvg-logo");
                var groupTitle = GetAttributeValue(line, "group-title") ?? DefaultCategory;

                var split = line.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length < 2)
                {
                    Console.Error.WriteLine("Invalid line: " + line);
                    continue;
                }

                var name = split[^1];

                if (i + 1 >= lines.Length)
                {
                    Console.Error.WriteLine("Invalid file format");
                    continue;
                }

                var url = lines[++i];

                if (!url.StartsWith("http"))
                {
                    Console.Error.WriteLine("Invalid URL: " + url + " on line " + i);
                    continue;
                }

                if (!categoryDict.TryGetValue(groupTitle, out var category))
                {
                    category = new Category(groupTitle);
                    categoryDict[groupTitle] = category;
                    categories.Add(category);
                }

                var channel = new Channel(category, name, url, tvgLogo, tvgId, tvgName);
                category.Channels.Add(channel);
            }
        }

        return categories;
    }

    public static async Task CreateM3UFileAsync(List<Category> categories, string filename, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        sb.AppendLine("#EXTM3U");

        foreach (var category in categories)
        {
            if (!category.IsIncluded || !category.Channels.Any(c => c.IsIncluded))
            {
                continue;
            }

            sb.AppendLine($"#EXTGRP:{category.Title}");

            foreach (var channel in category.Channels)
            {
                if (!channel.IsIncluded)
                {
                    continue;
                }

                sb.Append("#EXTINF:-1 ");

                if (channel.TvgId != null)
                {
                    sb.Append($"tvg-id=\"{channel.TvgId}\"");
                }

                if (channel.TvgName != null)
                {
                    sb.Append($" tvg-name=\"{channel.TvgName}\"");
                }

                if (channel.LogoUrl != null)
                {
                    sb.Append($" tvg-logo=\"{channel.LogoUrl}\"");
                }

                sb.AppendLine($" group-title=\"{category.Title}\",{channel.Name}");

                sb.AppendLine(channel.Url);
            }
        }

        await File.WriteAllTextAsync(filename, sb.ToString(), cancellationToken);
    }
}
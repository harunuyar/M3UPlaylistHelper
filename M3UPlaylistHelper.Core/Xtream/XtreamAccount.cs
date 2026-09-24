namespace M3UPlaylistHelper.Xtream;

using System.Globalization;
using System.Text.Json;

/// <summary>
/// Login details of an Xtream Codes panel, the API most IPTV providers use.
/// </summary>
public class XtreamAccount
{
    public const string OutputTs = "ts";
    public const string OutputHls = "m3u8";

    public string Server { get; }
    public string Username { get; }
    public string Password { get; }

    /// <summary>
    /// Stream format of the playlist URLs: <see cref="OutputTs"/> or <see cref="OutputHls"/>.
    /// </summary>
    public string Output { get; }

    public XtreamAccount(string server, string username, string password, string output = OutputTs)
    {
        Server = NormalizeServer(server);
        Username = username.Trim();
        Password = password.Trim();
        Output = output == OutputHls ? OutputHls : OutputTs;
    }

    /// <summary>
    /// Shown instead of the playlist URL, which contains the password.
    /// </summary>
    public string DisplayName => $"{Username} @ {new Uri(Server).Authority}";

    public string PlaylistUrl => $"{Server}/get.php?{Credentials}&type=m3u_plus&output={Output}";

    public string EpgUrl => $"{Server}/xmltv.php?{Credentials}";

    public string AccountInfoUrl => $"{Server}/player_api.php?{Credentials}";

    private string Credentials => $"username={Uri.EscapeDataString(Username)}&password={Uri.EscapeDataString(Password)}";

    /// <summary>
    /// Accepts "host:port", "http://host:port/" or a full panel URL such as "http://host:port/get.php?...".
    /// </summary>
    public static string NormalizeServer(string server)
    {
        server = server.Trim();

        if (!server.Contains("://", StringComparison.Ordinal))
        {
            server = "http://" + server;
        }

        if (!Uri.TryCreate(server, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new FormatException("The server address is not valid. Use the form http://example.com:8080");
        }

        // Keep a sub-path if the panel lives in one, but drop a script name such as /get.php
        var path = uri.AbsolutePath;
        if (path.EndsWith(".php", StringComparison.OrdinalIgnoreCase))
        {
            path = path[..path.LastIndexOf('/')];
        }

        return $"{uri.Scheme}://{uri.Authority}{path.TrimEnd('/')}";
    }

    public async Task<XtreamAccountInfo> GetAccountInfoAsync(CancellationToken cancellationToken)
    {
        using var response = await Http.Client.GetAsync(AccountInfoUrl, cancellationToken);
        Http.EnsureSuccess(response);
        return XtreamAccountInfo.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
    }
}

public record XtreamAccountInfo(bool IsAuthenticated, string? Status, DateTime? ExpiresAt, int? MaxConnections, int? ActiveConnections, bool IsTrial)
{
    /// <summary>
    /// Parses the player_api.php response. Panels are inconsistent about types (numbers as strings, nulls, "0"),
    /// so everything is read leniently.
    /// </summary>
    public static XtreamAccountInfo Parse(string json)
    {
        using var document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("user_info", out var userInfo) ||
            userInfo.ValueKind != JsonValueKind.Object)
        {
            return new XtreamAccountInfo(false, null, null, null, null, false);
        }

        var expiresAt = ReadLong(userInfo, "exp_date") is long seconds and > 0
            ? DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime
            : (DateTime?)null;

        return new XtreamAccountInfo(
            ReadLong(userInfo, "auth") == 1,
            ReadString(userInfo, "status"),
            expiresAt,
            (int?)ReadLong(userInfo, "max_connections"),
            (int?)ReadLong(userInfo, "active_cons"),
            ReadLong(userInfo, "is_trial") == 1);
    }

    private static string? ReadString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null ? value.ToString() : null;

    private static long? ReadLong(JsonElement element, string name) =>
        long.TryParse(ReadString(element, name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : null;
}

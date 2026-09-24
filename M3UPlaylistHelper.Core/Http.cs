namespace M3UPlaylistHelper;

using System.Net;

/// <summary>
/// The HTTP client shared by playlist, EPG and provider requests.
/// </summary>
public static class Http
{
    public static readonly HttpClient Client = CreateClient();

    private static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromMinutes(2),
        };

        // Some providers block requests without a user agent
        client.DefaultRequestHeaders.UserAgent.ParseAdd("M3UPlaylistHelper");
        return client;
    }

    /// <summary>
    /// Like <see cref="HttpResponseMessage.EnsureSuccessStatusCode"/>, with a message that reads well in a dialog.
    /// </summary>
    public static void EnsureSuccess(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"The server returned {(int)response.StatusCode} ({response.ReasonPhrase}).", null, response.StatusCode);
        }
    }
}

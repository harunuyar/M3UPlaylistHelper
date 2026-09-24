namespace M3UPlaylistHelper.Tests;

using M3UPlaylistHelper.Xtream;

public class XtreamTests
{
    [Theory]
    [InlineData("example.com:8080", "http://example.com:8080")]
    [InlineData("http://example.com:8080/", "http://example.com:8080")]
    [InlineData("https://example.com/get.php?username=a&password=b", "https://example.com")]
    [InlineData("http://example.com/panel/", "http://example.com/panel")]
    public void NormalizesServer(string input, string expected)
    {
        Assert.Equal(expected, XtreamAccount.NormalizeServer(input));
    }

    [Fact]
    public void RejectsInvalidServer()
    {
        Assert.Throws<FormatException>(() => XtreamAccount.NormalizeServer("ftp://example.com"));
    }

    [Fact]
    public void BuildsUrls()
    {
        var account = new XtreamAccount("example.com:8080", "user", "p&ss", XtreamAccount.OutputHls);

        Assert.Equal("http://example.com:8080/get.php?username=user&password=p%26ss&type=m3u_plus&output=m3u8", account.PlaylistUrl);
        Assert.Equal("http://example.com:8080/xmltv.php?username=user&password=p%26ss", account.EpgUrl);
        Assert.Equal("user @ example.com:8080", account.DisplayName);
    }

    [Fact]
    public void ParsesAccountInfoWithStringNumbers()
    {
        var info = XtreamAccountInfo.Parse("""
            {"user_info":{"auth":1,"status":"Active","exp_date":"1798761600","is_trial":"0","active_cons":"0","max_connections":"2"},"server_info":{}}
            """);

        Assert.True(info.IsAuthenticated);
        Assert.Equal("Active", info.Status);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1798761600).LocalDateTime, info.ExpiresAt);
        Assert.Equal(2, info.MaxConnections);
        Assert.False(info.IsTrial);
    }

    [Fact]
    public void ParsesFailedLoginAndUnlimitedAccount()
    {
        Assert.False(XtreamAccountInfo.Parse("""{"user_info":{"auth":0}}""").IsAuthenticated);
        Assert.False(XtreamAccountInfo.Parse("[]").IsAuthenticated);
        Assert.Null(XtreamAccountInfo.Parse("""{"user_info":{"auth":1,"exp_date":null}}""").ExpiresAt);
    }
}

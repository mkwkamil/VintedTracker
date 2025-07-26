using VintedTracker.Model;
using Timer = System.Timers.Timer;
using PuppeteerSharp;

namespace VintedTracker.Core;

public class VintedClient
{
    private readonly string _chromePath;
    private readonly Timer _timer;
    private VintedSession _currentSession = new();

    public VintedClient(string chromePath, double intervalMinutes = 20)
    {
        _chromePath = chromePath;
        _timer = new Timer(intervalMinutes * 60 * 1000);
        _timer.Elapsed += async (_, _) => await RefreshSessionAsync();
        _timer.AutoReset = true;
    }

    public async Task StartAsync()
    {
        await RefreshSessionAsync();
        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    public VintedSession GetCurrentSession() => _currentSession;

    private async Task RefreshSessionAsync()
    {
        Console.WriteLine("🔁 Refreshing session...");

        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = _chromePath,
            Args = new[]
            {
                "--no-sandbox", 
                "--disable-setuid-sandbox", 
                "--incognito"
            }
        });

        var page = await browser.NewPageAsync();

        await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
        {
            ["Accept-Language"] = "pl"
        });

        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        );

        await page.GoToAsync("https://www.vinted.pl");
        await page.WaitForSelectorAsync("body");

        var cookies = await page.GetCookiesAsync();
        await browser.CloseAsync();

        var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        var accessToken = cookies.FirstOrDefault(c => c.Name == "access_token_web")?.Value ?? "";

        _currentSession = new VintedSession
        {
            CookieHeader = cookieHeader,
            AccessToken = accessToken
        };

        Console.WriteLine("✅ Session refreshed.");
    }
}
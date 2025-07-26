using VintedTracker.Model;
using Timer = System.Timers.Timer;
using PuppeteerSharp;

namespace VintedTracker.Core;

// Handles Vinted session management using a headless browser to extract cookies and tokens
public class VintedClient
{
    private readonly string _chromePath;
    private readonly Timer _timer;
    private VintedSession _currentSession = new();

    // Constructor initializes the client with a path to the Chrome binary and refresh interval
    public VintedClient(string chromePath, double intervalMinutes = 20)
    {
        _chromePath = chromePath;
        _timer = new Timer(intervalMinutes * 60 * 1000);
        _timer.Elapsed += async (_, _) => await RefreshSessionAsync(); // Auto-refresh session
        _timer.AutoReset = true;
    }

    // Starts the initial session and begins the refresh timer
    public async Task StartAsync()
    {
        await RefreshSessionAsync();
        _timer.Start();
    }

    // Stops the periodic session refresh
    public void Stop() => _timer.Stop();

    // Returns the current session with cookies and token
    public VintedSession GetCurrentSession() => _currentSession;

    // Refreshes the session by launching a headless browser and retrieving cookies
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

        // Set headers and user agent to mimic a real user
        await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
        {
            ["Accept-Language"] = "pl"
        });

        await page.SetUserAgentAsync(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
        );

        // Navigate to Vinted and wait until body is loaded
        await page.GoToAsync("https://www.vinted.pl");
        await page.WaitForSelectorAsync("body");

        var cookies = await page.GetCookiesAsync();
        await browser.CloseAsync();

        // Build headers for API usage
        var cookieHeader = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
        var accessToken = cookies.FirstOrDefault(c => c.Name == "access_token_web")?.Value ?? "";

        // Store the refreshed session
        _currentSession = new VintedSession
        {
            CookieHeader = cookieHeader,
            AccessToken = accessToken
        };

        Console.WriteLine("✅ Session refreshed.");
    }
}
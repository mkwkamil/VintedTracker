using System.Text.Json;
using VintedTracker.Api;
using VintedTracker.Core;
using VintedTracker.Model;
using VintedTracker.Services;

// Path to the Chrome browser used by PuppeteerSharp
var chromePath = "/Users/kamilporebski/Downloads/chrome-mac-arm64/Google Chrome for Testing.app/Contents/MacOS/Google Chrome for Testing";

// Initialize Vinted client with the given Chrome path
var vintedClient = new VintedClient(chromePath);
await vintedClient.StartAsync(); // Start the browser and perform login

// Create an API client that uses the Vinted session
var apiClient = new VintedApiClient(vintedClient);

// Read Telegram configuration from the JSON file
var telegramSettings = JsonSerializer.Deserialize<TelegramSettings>(
    File.ReadAllText("Config/TelegramConfig.json")
)!;

// Initialize the Telegram notifier with loaded settings
var notifier = new TelegramNotifier(telegramSettings);

// Read tracker configurations (URLs and intervals) from the JSON file
var trackerConfigs = JsonSerializer.Deserialize<List<TrackerConfigEntry>>(
    File.ReadAllText("Config/TrackerConfig.json")
)!;

// Create and run the tracker manager that launches workers for each config
var manager = new VintedTrackerManager(apiClient, notifier);
await manager.StartAllAsync(trackerConfigs); // Run all workers asynchronously
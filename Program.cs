using System.Text.Json;
using VintedTracker;
using VintedTracker.Api;
using VintedTracker.Core;
using VintedTracker.Model;

var chromePath = "/Users/kamilporebski/Downloads/chrome-mac-arm64/Google Chrome for Testing.app/Contents/MacOS/Google Chrome for Testing";

var vintedClient = new VintedClient(chromePath);
await vintedClient.StartAsync();

var apiClient = new VintedApiClient(vintedClient);

var telegramSettings = JsonSerializer.Deserialize<TelegramSettings>(File.ReadAllText("Config/TelegramConfig.json"))!;
var notifier = new TelegramNotifier(telegramSettings);

var trackerConfigs = JsonSerializer.Deserialize<List<TrackerConfigEntry>>(File.ReadAllText("Config/TrackerConfig.json"))!;

var manager = new VintedTrackerManager(apiClient, notifier);
await manager.StartAllAsync(trackerConfigs);
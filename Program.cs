using VintedTracker;

var chromePath = "/Users/kamilporebski/Downloads/chrome-mac-arm64/Google Chrome for Testing.app/Contents/MacOS/Google Chrome for Testing";

var vintedClient = new VintedClient(chromePath);
await vintedClient.StartAsync();

var apiClient = new VintedApiClient(vintedClient);
var tracker = new VintedTrackerService(apiClient);

await tracker.StartTrackingAsync();
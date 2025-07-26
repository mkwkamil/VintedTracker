using VintedTracker.Api;
using VintedTracker.Model;
using VintedTracker.Services;

namespace VintedTracker.Core;

// Handles tracking for a single Vinted config entry
public class VintedTrackerWorker(VintedApiClient apiClient, TelegramNotifier notifier, TrackerConfigEntry config)
{
    private readonly ItemMemory _memory = new();

    // Runs the tracking loop for this worker
    public async Task RunAsync()
    {
        Console.WriteLine($"🚀 Starting tracker for: {config.Title}");

        // Perform initial fetch and remember existing items
        var initialItems = await apiClient.GetParsedItemsAsync(config.Url);
        foreach (var item in initialItems)
            _memory.Add(item.Id);

        Console.WriteLine($"✅ {config.Title} – Initial items loaded: {_memory.Count}");

        // Infinite tracking loop
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(config.IntervalSeconds));

            Console.WriteLine($"🔄 Checking {config.Title}...");

            // Fetch latest items and detect new ones
            var newItems = await apiClient.GetParsedItemsAsync(config.Url);
            var newOnes = newItems.Where(item => !_memory.Contains(item.Id)).ToList();

            if (newOnes.Count == 0)
            {
                Console.WriteLine($"⏳ {config.Title} – No new items.");
                continue;
            }

            // Notify via Telegram and update memory
            foreach (var item in newOnes)
            {
                _memory.Add(item.Id);
                await notifier.SendMessageAsync(item);
            }

            Console.WriteLine($"📦 {config.Title} – {newOnes.Count} new items sent to Telegram.");
        }
        // ReSharper disable once FunctionNeverReturns
    }
}
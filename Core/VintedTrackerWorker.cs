using VintedTracker.Api;
using VintedTracker.Model;

namespace VintedTracker.Core;

public class VintedTrackerWorker(VintedApiClient apiClient, TelegramNotifier notifier, TrackerConfigEntry config)
{
    private readonly ItemMemory _memory = new();

    public async Task RunAsync()
    {
        Console.WriteLine($"🚀 Starting tracker for: {config.Title}");

        var initialItems = await apiClient.GetParsedItemsAsync(config.Url);
        foreach (var item in initialItems)
            _memory.Add(item.Id);

        Console.WriteLine($"✅ {config.Title} – Initial items loaded: {_memory.Count}");

        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(config.IntervalSeconds));

            Console.WriteLine($"🔄 Checking {config.Title}...");
            var newItems = await apiClient.GetParsedItemsAsync(config.Url);
            var newOnes = newItems.Where(item => !_memory.Contains(item.Id)).ToList();

            if (newOnes.Count == 0)
            {
                Console.WriteLine($"⏳ {config.Title} – No new items.");
                continue;
            }

            foreach (var item in newOnes)
            {
                _memory.Add(item.Id);
                await notifier.SendMessageAsync(item);
            }

            Console.WriteLine($"📦 {config.Title} – {newOnes.Count} new items sent to Telegram.");
        }
    }
}
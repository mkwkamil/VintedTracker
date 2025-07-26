using VintedTracker.Api;
using VintedTracker.Model;
using VintedTracker.Services;

namespace VintedTracker.Core;

// Responsible for launching all tracking workers based on config entries
public class VintedTrackerManager(VintedApiClient apiClient, TelegramNotifier notifier)
{
    // Starts all tracker workers concurrently
    public async Task StartAllAsync(List<TrackerConfigEntry> trackerConfigs)
    {
        var tasks = trackerConfigs.Select(cfg =>
        {
            // Create a worker for each configuration entry
            var worker = new VintedTrackerWorker(apiClient, notifier, cfg);

            // Start worker task
            return worker.RunAsync();
        });

        // Await all workers to run in parallel
        await Task.WhenAll(tasks);
    }
}
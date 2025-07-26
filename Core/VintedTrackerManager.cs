using VintedTracker.Api;
using VintedTracker.Model;

namespace VintedTracker.Core;

public class VintedTrackerManager(VintedApiClient apiClient, TelegramNotifier notifier)
{
    public async Task StartAllAsync(List<TrackerConfigEntry> trackerConfigs)
    {
        var tasks = trackerConfigs.Select(cfg =>
        {
            var worker = new VintedTrackerWorker(apiClient, notifier, cfg);
            
            return worker.RunAsync();
        });

        await Task.WhenAll(tasks);
    }
}
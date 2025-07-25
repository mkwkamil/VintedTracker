namespace VintedTracker;

public class VintedTrackerService(VintedApiClient apiClient)
{
    private readonly ItemMemory _memory = new();
    private readonly TelegramNotifier _notifier = new();
    
    public async Task StartTrackingAsync()
    {
        Console.WriteLine("🔍 Starting initial fetch...");
        var initialItems = await apiClient.GetParsedItemsAsync();
        
        foreach (var item in initialItems)
            _memory.Add(item.Id);

        Console.WriteLine($"✅ Initial items loaded: {_memory.Count}");
        
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));

            Console.WriteLine("🔄 Checking for new items...");
            var newItems = await apiClient.GetParsedItemsAsync();

            var newOnes = newItems.Where(item => !_memory.Contains(item.Id)).ToList();

            if (newOnes.Count == 0)
            {
                Console.WriteLine("⏳ No new items found.");
                continue;
            }

            foreach (var item in newOnes)
            {
                _memory.Add(item.Id);
                await _notifier.SendMessageAsync(item);
            }
        }
    }
}
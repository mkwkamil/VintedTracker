namespace VintedTracker.Core;

// A fixed-size memory structure to track recently seen item IDs
public class ItemMemory
{
    private readonly LinkedList<long> _queue = new(); // Maintains insertion order
    private readonly HashSet<long> _set = new();      // Enables fast lookup
    private const int MaxSize = 1000;                 // Maximum number of items to store

    public int Count => _set.Count; // Current number of stored IDs

    // Checks if a given ID is already stored
    public bool Contains(long id) => _set.Contains(id);

    // Adds a new ID, evicting the oldest if the memory is full
    public void Add(long id)
    {
        if (_set.Contains(id)) return; // Ignore duplicates

        if (_set.Count >= MaxSize)
        {
            var oldest = _queue.First!;    // Get the oldest item
            _queue.RemoveFirst();          // Remove it from the queue
            _set.Remove(oldest.Value);     // Remove it from the lookup set
        }

        _queue.AddLast(id); // Add new ID to the end of the queue
        _set.Add(id);       // And to the lookup set
    }
}
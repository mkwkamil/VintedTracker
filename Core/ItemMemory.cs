namespace VintedTracker.Core;

public class ItemMemory
{
    private readonly LinkedList<long> _queue = new();
    private readonly HashSet<long> _set = new();
    private const int MaxSize = 1000;

    public int Count => _set.Count;

    public bool Contains(long id) => _set.Contains(id);

    public void Add(long id)
    {
        if (_set.Contains(id)) return;

        if (_set.Count >= MaxSize)
        {
            var oldest = _queue.First!;
            _queue.RemoveFirst();
            _set.Remove(oldest.Value);
        }

        _queue.AddLast(id);
        _set.Add(id);
    }
}
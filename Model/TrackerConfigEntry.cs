namespace VintedTracker.Model;

public class TrackerConfigEntry
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int IntervalSeconds { get; set; }
}
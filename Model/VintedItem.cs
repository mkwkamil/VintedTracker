namespace VintedTracker.Model;

public class VintedItem
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Size { get; set; } = "";
    public string Price { get; set; } = "";
    public string TotalPrice { get; set; } = "";
    public string Url { get; set; } = "";
    public string PhotoUrl { get; set; } = "";
}
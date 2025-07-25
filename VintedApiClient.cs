using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using VintedTracker.Model;

namespace VintedTracker;

public class VintedApiClient(VintedClient vintedClient)
{
    private async Task<string> FetchLatestItemsAsync()
    {
        var session = vintedClient.GetCurrentSession();
        
        using var handler = new HttpClientHandler();
        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        client.DefaultRequestHeaders.Add("Cookie", session.CookieHeader);
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        client.DefaultRequestHeaders.Add("Accept-Language", "pl");
        
        var url = "https://www.vinted.pl/api/v2/catalog/items" +
                  "?catalog_ids[]=79" +
                  "&size_ids[]=209&size_ids[]=208&size_ids[]=210&size_ids[]=211" +
                  "&price_to=100&currency=PLN" +
                  "&order=newest_first&page=1";
        
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Request failed: {(int)response.StatusCode}");
            return "[]";
        }
        
        var content = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"📥 Status: {(int)response.StatusCode}");
        return content;
    }

    public async Task<List<VintedItem>> GetParsedItemsAsync()
    {
        var json = await FetchLatestItemsAsync();
        
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("items", out var items))
            return new();
        
        var result = new List<VintedItem>();
        
        foreach (var item in items.EnumerateArray())
        {
            result.Add(new VintedItem
            {
                Id = item.GetProperty("id").GetInt64(),
                Title = item.GetProperty("title").GetString() ?? "No Title",
                Brand = item.GetProperty("brand_title").GetString() ?? "Unknown",
                Size = item.GetProperty("size_title").GetString() ?? "Unknown",
                Price = item.GetProperty("price").GetProperty("amount").GetString() ?? "0",
                TotalPrice = item.GetProperty("total_item_price").GetProperty("amount").GetString() ?? "",
                Url = item.GetProperty("url").GetString() ?? "",
                PhotoUrl = item.GetProperty("photo").GetProperty("thumbnails").EnumerateArray()
                    .First(t => t.GetProperty("type").GetString() == "thumb310x430").GetProperty("url")
                    .GetString() ?? ""
            });
        }

        return result;
    }
}
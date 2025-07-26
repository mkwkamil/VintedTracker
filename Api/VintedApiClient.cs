using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using VintedTracker.Core;
using VintedTracker.Model;

namespace VintedTracker.Api;

public class VintedApiClient(VintedClient vintedClient)
{
    public async Task<List<VintedItem>> GetParsedItemsAsync(string url)
    {
        var session = vintedClient.GetCurrentSession();
        
        using var handler = new HttpClientHandler();
        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        client.DefaultRequestHeaders.Add("Cookie", session.CookieHeader);
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        client.DefaultRequestHeaders.Add("Accept-Language", "pl");

        try
        {
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Request failed ({(int)response.StatusCode}): {url}");
                return [];
            }
            
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("items", out var items))
                return [];
            
            return items.EnumerateArray().Select(item => new VintedItem
            {
                Id = item.GetProperty("id").GetInt64(),
                Title = item.GetProperty("title").GetString() ?? "No Title",
                Brand = item.GetProperty("brand_title").GetString() ?? "Unknown",
                Size = item.GetProperty("size_title").GetString() ?? "Unknown",
                Price = item.GetProperty("price").GetProperty("amount").GetString() ?? "0",
                TotalPrice = item.GetProperty("total_item_price").GetProperty("amount").GetString() ?? "",
                Url = item.GetProperty("url").GetString() ?? "",
                PhotoUrl = item.GetProperty("photo").GetProperty("thumbnails")
                    .EnumerateArray().First(t => t.GetProperty("type").GetString() == "thumb310x430")
                    .GetProperty("url").GetString() ?? ""
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception while fetching items: {ex.Message}");
            return [];
        }
    }
}
using System.Text;
using System.Text.Json;
using VintedTracker.Model;

namespace VintedTracker;

public class TelegramNotifier(TelegramSettings settings)
{
    private readonly string _botToken = settings.BotToken;
    private readonly string[] _chatIds = settings.ChatIds;
    private static readonly HttpClient Http = new();

    public async Task SendMessageAsync(VintedItem item)
    {
        var url = $"https://api.telegram.org/bot{_botToken}/sendPhoto";

        var caption =
            $"Title: *{item.Title}*\n" +
            $"Brand: *{item.Brand}*\n" +
            $"Size: *{item.Size}*\n" +
            $"Price: *{item.Price} PLN* ({item.TotalPrice} PLN)\n";

        var inlineKeyboard = new
        {
            inline_keyboard = new[]
            {
                new[]
                {
                    new { text = "Buy Now", url = item.Url }
                }
            }
        };

        foreach (var chatId in _chatIds)
        {
            try
            {
                var payload = new
                {
                    chat_id = chatId,
                    photo = item.PhotoUrl,
                    caption,
                    parse_mode = "markdown",
                    reply_markup = inlineKeyboard
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await Http.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Telegram send failed: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception while sending message to {chatId}: {ex.Message}");
            }
        }
    }
}
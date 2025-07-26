using System.Text;
using System.Text.Json;
using VintedTracker.Model;

namespace VintedTracker.Services;

// Handles sending messages to Telegram
public class TelegramNotifier(TelegramSettings settings)
{
    private readonly string _botToken = settings.BotToken; // Telegram bot token from config
    private readonly string[] _chatIds = settings.ChatIds; // List of chat IDs to send messages to
    private static readonly HttpClient Http = new();       // Shared HTTP client instance

    public async Task SendMessageAsync(VintedItem item)
    {
        var url = $"https://api.telegram.org/bot{_botToken}/sendPhoto"; // Telegram API endpoint

        var caption = // Message body formatted in Markdown
            $"Title: *{item.Title}*\n" +
            $"Brand: *{item.Brand}*\n" +
            $"Size: *{item.Size}*\n" +
            $"Price: *{item.Price} PLN* ({item.TotalPrice} PLN)\n";

        var inlineKeyboard = new // Inline button for opening the item URL
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
                var payload = new // Telegram message payload
                {
                    chat_id = chatId,
                    photo = item.PhotoUrl,
                    caption,
                    parse_mode = "markdown",
                    reply_markup = inlineKeyboard
                };

                var content = new StringContent( // JSON body for HTTP POST
                    JsonSerializer.Serialize(payload), 
                    Encoding.UTF8, 
                    "application/json"
                );

                var response = await Http.PostAsync(url, content); // Send POST request to Telegram API

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(); // Read error message
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
namespace VintedTracker.Model;

public class TelegramSettings
{
    public string BotToken { get; set; } = string.Empty;
    public string[] ChatIds { get; set; } = [];
}
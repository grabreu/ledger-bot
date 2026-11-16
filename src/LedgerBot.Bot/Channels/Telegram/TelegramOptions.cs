namespace LedgerBot.Bot.Channels.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    public string BotToken { get; set; } = "";

    public string WebhookUrl { get; set; } = "";

    public string WebhookSecret { get; set; } = "";
}

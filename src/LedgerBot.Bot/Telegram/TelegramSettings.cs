namespace LedgerBot.Bot.Telegram;

public sealed class TelegramSettings
{
    public const string SectionName = "Telegram";

    [Required] public string BotToken { get; set; } = string.Empty;
    [Required] public string WebhookUrl { get; set; } = string.Empty;
    [Required] public string WebhookSecret { get; set; } = string.Empty;
}

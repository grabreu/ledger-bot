using System.ComponentModel.DataAnnotations;

namespace LedgerBot.Bot.Channels.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    [Required]
    public string BotToken { get; set; } = "";

    [Required]
    public string WebhookUrl { get; set; } = "";

    [Required]
    public string WebhookSecret { get; set; } = "";
}

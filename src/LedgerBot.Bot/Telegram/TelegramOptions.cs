using System.ComponentModel.DataAnnotations;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramOptions
{
    public const string SectionName = "Telegram";

    [Required] public string BotToken { get; set; } = string.Empty;
}

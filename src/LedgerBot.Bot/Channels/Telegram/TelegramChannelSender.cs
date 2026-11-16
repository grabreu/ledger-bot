using System.Globalization;

namespace LedgerBot.Bot.Channels.Telegram;

public sealed class TelegramChannelSender(ITelegramBotClient bot) : IChannelSender
{
    public async Task SendTextAsync(string chatId, string text, CancellationToken cancellationToken = default)
    {
        await bot.SendMessage(long.Parse(chatId, CultureInfo.InvariantCulture), text, cancellationToken: cancellationToken);
    }
}

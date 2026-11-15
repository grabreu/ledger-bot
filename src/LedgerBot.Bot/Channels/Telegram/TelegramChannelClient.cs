using System.Globalization;
using Telegram.Bot;

namespace LedgerBot.Bot.Channels.Telegram;

public sealed class TelegramChannelClient(ITelegramBotClient bot) : IChannelClient
{
    public async Task SendTextAsync(string chatId, string text, CancellationToken cancellationToken = default)
    {
        await bot.SendMessage(long.Parse(chatId, CultureInfo.InvariantCulture), text, cancellationToken: cancellationToken);
    }
}

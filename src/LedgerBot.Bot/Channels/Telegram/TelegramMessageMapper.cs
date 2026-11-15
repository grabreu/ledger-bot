using System.Globalization;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Channels.Telegram;

public static class TelegramMessageMapper
{
    public const string ChannelName = "telegram";

    public static IncomingMessage? ToIncomingMessage(Message message)
    {
        if (message.From is null || string.IsNullOrWhiteSpace(message.Text))
        {
            return null;
        }

        var user = new ChannelUser(ChannelName, message.From.Id.ToString(CultureInfo.InvariantCulture));
        var chatId = message.Chat.Id.ToString(CultureInfo.InvariantCulture);

        return new IncomingMessage(user, chatId, message.Text);
    }
}

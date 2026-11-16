using System.Globalization;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Channels.Telegram;

public static class TelegramMessageExtensions
{
    private const string ChannelName = "telegram";

    extension(Message message)
    {
        public IncomingMessage? ToIncomingMessage()
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
}

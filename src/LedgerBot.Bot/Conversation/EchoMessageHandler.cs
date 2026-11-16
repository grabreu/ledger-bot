using LedgerBot.Bot.Channels;

namespace LedgerBot.Bot.Conversation;

// Provisional: proves the channel round trip until the real handlers exist.
public sealed class EchoMessageHandler(IChannelSender channel) : IIncomingMessageHandler
{
    public Task HandleAsync(IncomingMessage message, CancellationToken cancellationToken = default)
    {
        return channel.SendTextAsync(message.ChatId, $"recebi: {message.Text}", cancellationToken);
    }
}

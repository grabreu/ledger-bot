namespace LedgerBot.Bot.Channels;

// Provisional: proves the channel round trip until the real handlers exist.
public sealed class EchoMessageHandler(IChannelClient channel) : IIncomingMessageHandler
{
    public Task HandleAsync(IncomingMessage message, CancellationToken cancellationToken = default)
    {
        return channel.SendTextAsync(message.ChatId, $"recebi: {message.Text}", cancellationToken);
    }
}

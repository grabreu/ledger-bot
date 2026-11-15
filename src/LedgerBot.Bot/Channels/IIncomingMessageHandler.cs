namespace LedgerBot.Bot.Channels;

public interface IIncomingMessageHandler
{
    Task HandleAsync(IncomingMessage message, CancellationToken cancellationToken = default);
}

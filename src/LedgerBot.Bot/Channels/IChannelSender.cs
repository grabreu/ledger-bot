namespace LedgerBot.Bot.Channels;

public interface IChannelSender
{
    Task SendTextAsync(string chatId, string text, CancellationToken cancellationToken = default);
}

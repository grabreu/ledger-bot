namespace LedgerBot.Bot.Channels;

public interface IChannelClient
{
    Task SendTextAsync(string chatId, string text, CancellationToken cancellationToken = default);
}

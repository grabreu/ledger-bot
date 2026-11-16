using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LedgerBot.Bot.Channels.Telegram.Polling;

public sealed class TelegramPollingService(
    TelegramBotClient bot,
    IServiceScopeFactory scopeFactory,
    ILogger<TelegramPollingService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await bot.DeleteWebhook(cancellationToken: cancellationToken);

        bot.OnMessage += OnMessage;
        bot.OnError += OnError;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        bot.OnMessage -= OnMessage;
        bot.OnError -= OnError;

        return Task.CompletedTask;
    }

    private async Task OnMessage(Message message, UpdateType type)
    {
        var incoming = message.ToIncomingMessage();
        if (incoming is null)
        {
            return;
        }

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<IIncomingMessageHandler>();
            await handler.HandleAsync(incoming);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to handle a Telegram message");
        }
    }

    private Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Telegram polling error from {Source}", source);

        return Task.CompletedTask;
    }
}

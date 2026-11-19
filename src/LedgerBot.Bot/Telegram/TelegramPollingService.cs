using Telegram.Bot.Polling;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramPollingService(
    TelegramBotClient bot,
    IServiceScopeFactory scopeFactory,
    ILogger<TelegramPollingService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await bot.DeleteWebhook(cancellationToken: cancellationToken);

        bot.OnUpdate += OnUpdate;
        bot.OnError += OnError;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        bot.OnUpdate -= OnUpdate;
        bot.OnError -= OnError;

        return Task.CompletedTask;
    }

    private async Task OnUpdate(Update update)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<TelegramUpdateHandler>();
        await handler.HandleAsync(update);
    }

    private Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Telegram polling error from {Source}", source);

        return Task.CompletedTask;
    }
}

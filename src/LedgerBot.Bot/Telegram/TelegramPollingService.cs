using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramPollingService(
    TelegramBotClient bot,
    IServiceScopeFactory scopeFactory,
    IHostApplicationLifetime lifetime,
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
        var dispatcher = scope.ServiceProvider.GetRequiredService<TelegramUpdateDispatcher>();
        await dispatcher.DispatchAsync(update, lifetime.ApplicationStopping);
    }

    private Task OnError(Exception exception, HandleErrorSource source)
    {
        logger.LogError(exception, "Telegram polling error from {Source}", source);

        return Task.CompletedTask;
    }
}

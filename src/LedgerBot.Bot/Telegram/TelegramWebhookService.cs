using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramWebhookService(ITelegramBotClient bot, IOptions<TelegramOptions> options) : IHostedService
{
    private readonly TelegramOptions _options = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var url = _options.WebhookUrl.TrimEnd('/') + TelegramWebhookEndpoint.Route;

        await bot.SetWebhook(
            url,
            secretToken: _options.WebhookSecret,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

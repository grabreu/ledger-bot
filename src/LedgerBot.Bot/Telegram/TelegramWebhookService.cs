using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramWebhookService(ITelegramBotClient bot, IOptions<TelegramOptions> options) : IHostedService
{
    private readonly TelegramOptions _options = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var url = _options.WebhookUrl.TrimEnd('/') + TelegramWebhookEndpoint.Route;

        await bot.SetWebhook(
            url,
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
            secretToken: _options.WebhookSecret,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

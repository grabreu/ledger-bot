using Telegram.Bot.Types.Enums;

namespace LedgerBot.Bot.Channels.Telegram;

public sealed class TelegramWebhookService(
    ITelegramBotClient bot,
    IOptions<TelegramOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var url = settings.WebhookUrl.TrimEnd('/') + TelegramWebhookEndpoint.Route;

        await bot.SetWebhook(
            url,
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
            secretToken: settings.WebhookSecret,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

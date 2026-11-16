namespace LedgerBot.Bot.Telegram;

public sealed class TelegramWebhookService(
    ITelegramBotClient bot,
    IOptions<TelegramSettings> options) : IHostedService
{
    private readonly TelegramSettings _settings = options.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var url = _settings.WebhookUrl.TrimEnd('/') + TelegramWebhookEndpoint.Route;

        await bot.SetWebhook(
            url,
            allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery],
            secretToken: _settings.WebhookSecret,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

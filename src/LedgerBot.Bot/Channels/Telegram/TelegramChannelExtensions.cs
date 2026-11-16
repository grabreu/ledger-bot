using LedgerBot.Bot.Channels.Telegram.Polling;
using LedgerBot.Bot.Channels.Telegram.Webhook;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace LedgerBot.Bot.Channels.Telegram;

public static class TelegramChannelExtensions
{
    public static IHostApplicationBuilder AddTelegramChannel(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName)
            .Validate(options => !string.IsNullOrWhiteSpace(options.BotToken), "Telegram:BotToken is required.")
            .Validate(
                options => options.Mode != TelegramMode.Webhook || !string.IsNullOrWhiteSpace(options.WebhookUrl),
                "Telegram:WebhookUrl is required in Webhook mode.")
            .Validate(
                options => options.Mode != TelegramMode.Webhook || IsValidSecret(options.WebhookSecret),
                "Telegram:WebhookSecret must be 1 to 256 characters of letters, digits, '_' or '-' in Webhook mode.")
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddSingleton<IChannelClient, TelegramChannelClient>();

        var mode = builder.Configuration.GetValue<TelegramMode>($"{TelegramOptions.SectionName}:Mode");
        if (mode == TelegramMode.Webhook)
        {
            builder.Services.AddHostedService<TelegramWebhookService>();
        }
        else
        {
            builder.Services.AddHostedService<TelegramPollingService>();
        }

        return builder;
    }

    public static WebApplication MapTelegramChannel(this WebApplication app)
    {
        if (app.Services.GetRequiredService<IOptions<TelegramOptions>>().Value.Mode == TelegramMode.Webhook)
        {
            app.MapTelegramWebhookEndpoint();
        }

        return app;
    }

    private static bool IsValidSecret(string secret)
    {
        return secret.Length is >= 1 and <= 256 && secret.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-');
    }
}

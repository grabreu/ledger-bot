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
            .Validate(options => !string.IsNullOrWhiteSpace(options.WebhookUrl), "Telegram:WebhookUrl is required.")
            .Validate(
                options => IsValidSecret(options.WebhookSecret),
                "Telegram:WebhookSecret must be 1 to 256 characters of letters, digits, '_' or '-'.")
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddSingleton<IChannelClient, TelegramChannelClient>();
        builder.Services.AddHostedService<TelegramWebhookService>();

        return builder;
    }

    private static bool IsValidSecret(string secret)
    {
        return secret.Length is >= 1 and <= 256 && secret.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-');
    }
}

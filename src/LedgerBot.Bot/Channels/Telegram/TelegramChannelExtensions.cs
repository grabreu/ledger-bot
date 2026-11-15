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
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddSingleton<IChannelClient, TelegramChannelClient>();
        builder.Services.AddHostedService<TelegramPollingService>();

        return builder;
    }
}

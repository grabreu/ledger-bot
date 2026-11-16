namespace LedgerBot.Bot.Channels.Telegram;

public static class TelegramChannelExtensions
{
    public static IHostApplicationBuilder AddTelegramChannel(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddSingleton<IChannelSender, TelegramChannelSender>();
        builder.Services.AddHostedService<TelegramWebhookService>();

        return builder;
    }
}

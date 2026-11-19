using LedgerBot.Bot.Telegram;

namespace LedgerBot.Bot;

public static class BotModuleExtensions
{
    public static IHostApplicationBuilder AddBotModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<TelegramOptions>()
            .BindConfiguration(TelegramOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddScoped<TelegramUpdateHandler>();

        var mode = builder.Configuration.GetSection(TelegramOptions.SectionName).Get<TelegramOptions>()?.Mode ?? TelegramMode.Webhook;

        if (mode == TelegramMode.Polling)
        {
            builder.Services.AddHostedService<TelegramPollingService>();
        }
        else
        {
            builder.Services.AddHostedService<TelegramWebhookService>();
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapBotModuleEndpoints(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<TelegramOptions>>().Value;

        if (options.Mode == TelegramMode.Webhook)
        {
            app.MapTelegramWebhookEndpoint();
        }

        return app;
    }
}

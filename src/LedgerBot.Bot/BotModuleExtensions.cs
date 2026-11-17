using LedgerBot.Bot.Features.InterpretMessage;
using LedgerBot.Bot.Telegram;

namespace LedgerBot.Bot;

public static class BotModuleExtensions
{
    public static IHostApplicationBuilder AddBotModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<TelegramSettings>()
            .BindConfiguration(TelegramSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramSettings>>().Value.BotToken));
        builder.Services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<TelegramBotClient>());
        builder.Services.AddHostedService<TelegramWebhookService>();

        builder.Services.AddScoped<InterpretMessageHandler>();

        return builder;
    }

    public static IEndpointRouteBuilder MapBotModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapTelegramWebhookEndpoint();

        return endpoints;
    }
}

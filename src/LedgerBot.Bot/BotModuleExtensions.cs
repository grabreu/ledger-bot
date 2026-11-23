using LedgerBot.Bot.Telegram;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

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

        builder.Services.AddScoped<TelegramUpdateDispatcher>();

        builder.Services.AddHostedService<TelegramPollingService>();

        return builder;
    }
}

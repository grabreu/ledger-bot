using LedgerBot.Bot.Data;
using LedgerBot.Bot.Telegram;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        builder.AddSqlServerDbContext<BotDbContext>(
            "LedgerBot",
            configureDbContextOptions: options => options.UseSqlServer(sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Bot")));

        builder.Services.AddScoped<ProcessedUpdateStore>();
        builder.Services.AddScoped<TelegramUpdateDispatcher>();

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

    public static async Task MigrateBotModuleDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        await context.Database.MigrateAsync();
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

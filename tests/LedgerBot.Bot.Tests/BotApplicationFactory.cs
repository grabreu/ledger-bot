using LedgerBot.Bot.Channels.Telegram;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LedgerBot.Bot.Tests;

public sealed class BotApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Telegram:BotToken", "test-token");
        builder.UseSetting("Telegram:WebhookUrl", "https://localhost");
        builder.UseSetting("Telegram:WebhookSecret", "test-secret");

        builder.ConfigureServices(services =>
        {
            var webhook = services.Where(descriptor => descriptor.ImplementationType == typeof(TelegramWebhookService)).ToList();
            foreach (var descriptor in webhook)
            {
                services.Remove(descriptor);
            }
        });
    }
}

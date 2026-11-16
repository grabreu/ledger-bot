using LedgerBot.Bot.Channels.Telegram.Polling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LedgerBot.Bot.Tests;

public sealed class BotApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Telegram:BotToken", "test-token");

        builder.ConfigureServices(services =>
        {
            var polling = services.Where(descriptor => descriptor.ImplementationType == typeof(TelegramPollingService)).ToList();
            foreach (var descriptor in polling)
            {
                services.Remove(descriptor);
            }
        });
    }
}

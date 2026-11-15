using System.Net;

namespace LedgerBot.Bot.Tests;

public class HealthEndpointTests(BotApplicationFactory factory) : IClassFixture<BotApplicationFactory>
{
    [Fact]
    public async Task Health_ShouldReturnOk()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

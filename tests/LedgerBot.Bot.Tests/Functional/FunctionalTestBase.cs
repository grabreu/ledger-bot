using LedgerBot.Bot.Data;
using LedgerBot.Bot.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerBot.Bot.Tests.Functional;

[Collection(nameof(FunctionalTestCollection))]
public abstract class FunctionalTestBase(FunctionalTestFixture fixture)
{
    protected static long NewUpdateId() => Random.Shared.NextInt64(1, long.MaxValue);

    protected async Task<TResult> WithStoreAsync<TResult>(Func<ProcessedUpdateStore, Task<TResult>> action)
    {
        using var scope = fixture.Services.CreateScope();
        return await action(scope.ServiceProvider.GetRequiredService<ProcessedUpdateStore>());
    }

    protected async Task<ProcessedUpdate?> FindAsync(long updateId)
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        return await dbContext.FindAsync<ProcessedUpdate>([updateId], TestContext.Current.CancellationToken);
    }
}

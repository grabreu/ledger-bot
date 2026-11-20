using LedgerBot.Ledger.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerBot.Ledger.Tests.Functional;

[Collection(nameof(FunctionalTestCollection))]
public abstract class FunctionalTestBase(FunctionalTestFixture fixture)
{
    protected static long NewChatId() => Random.Shared.NextInt64(1, long.MaxValue);

    protected async ValueTask<TResponse> SendAsync<TResponse>(ICommand<TResponse> command)
    {
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(command, TestContext.Current.CancellationToken);
    }

    protected async ValueTask<TResponse> SendAsync<TResponse>(IQuery<TResponse> query)
    {
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(query, TestContext.Current.CancellationToken);
    }

    public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        return await dbContext.FindAsync<TEntity>(keyValues);
    }

    protected async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        return await dbContext.Set<TEntity>().CountAsync();
    }
}

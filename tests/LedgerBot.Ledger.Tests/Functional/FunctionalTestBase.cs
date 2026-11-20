using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerBot.Ledger.Tests.Functional;

[Collection(nameof(FunctionalTestCollection))]
public abstract class FunctionalTestBase(FunctionalTestFixture fixture)
{
    protected async ValueTask<TResponse> SendAsync<TResponse>(ICommand<TResponse> command)
    {
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(command, TestContext.Current.CancellationToken);
    }
}

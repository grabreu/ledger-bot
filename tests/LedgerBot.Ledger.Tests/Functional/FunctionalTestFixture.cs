using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace LedgerBot.Ledger.Tests.Functional;

public sealed class FunctionalTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private IHost? _host;

    public IServiceProvider Services => _host!.Services;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var builder = Host.CreateApplicationBuilder();
        builder.Configuration["ConnectionStrings:LedgerBot"] = _container.GetConnectionString();
        builder.AddLedgerModuleServices();

        _host = builder.Build();
        await _host.MigrateLedgerModuleDatabaseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _host?.Dispose();
        await _container.DisposeAsync();
    }
}

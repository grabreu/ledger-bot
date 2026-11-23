using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace LedgerBot.Bot.Tests.Functional;

public sealed class FunctionalTestFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private IHost? _host;

    public IServiceProvider Services => _host!.Services;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings { EnvironmentName = Environments.Production });
        builder.Configuration["ConnectionStrings:LedgerBot"] = _container.GetConnectionString();
        builder.Configuration["Telegram:BotToken"] = "1:test";
        builder.Configuration["Telegram:Mode"] = "Polling";
        builder.Services.AddSingleton(TimeProvider.System);
        builder.AddBotModuleServices();

        _host = builder.Build();
        await _host.MigrateBotModuleDatabaseAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _host?.Dispose();
        await _container.DisposeAsync();
    }
}

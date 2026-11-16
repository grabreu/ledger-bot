using LedgerBot.Ledger.Data;

namespace LedgerBot.Ledger;

public static class LedgerModuleExtensions
{
    public static IHostApplicationBuilder AddLedgerModuleServices(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<LedgerDbContext>(
            "ledgerbot",
            configureDbContextOptions: options => options.UseSqlServer(sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Ledger")));

        return builder;
    }

    public static async Task EnsureLedgerModuleDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        await context.Database.MigrateAsync();
    }
}

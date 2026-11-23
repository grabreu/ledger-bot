using System.Reflection;
using FluentValidation;
using LedgerBot.Ledger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace LedgerBot.Ledger;

public static class LedgerModuleExtensions
{
    public static IHostApplicationBuilder AddLedgerModuleServices(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<LedgerDbContext>(
            "LedgerBot",
            configureDbContextOptions: options => options.UseSqlServer(sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Ledger")));

        builder.Services.TryAddSingleton(TimeProvider.System);

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        });

        return builder;
    }

    public static async Task MigrateLedgerModuleDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LedgerDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}

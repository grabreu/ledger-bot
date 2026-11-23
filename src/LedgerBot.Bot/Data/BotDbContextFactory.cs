using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LedgerBot.Bot.Data;

public sealed class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=LedgerBot;Trusted_Connection=True;MultipleActiveResultSets=true",
            sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Bot"));

        return new BotDbContext(optionsBuilder.Options);
    }
}

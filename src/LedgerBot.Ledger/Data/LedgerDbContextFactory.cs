namespace LedgerBot.Ledger.Data;

public sealed class LedgerDbContextFactory : IDesignTimeDbContextFactory<LedgerDbContext>
{
    public LedgerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LedgerDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=ledgerbot;Trusted_Connection=True;MultipleActiveResultSets=true",
            sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Ledger"));

        return new LedgerDbContext(optionsBuilder.Options);
    }
}

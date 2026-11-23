using System.Reflection;
using LedgerBot.Bot.Domain;
using Microsoft.EntityFrameworkCore;

namespace LedgerBot.Bot.Data;

public sealed class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<ProcessedUpdate> ProcessedUpdates => Set<ProcessedUpdate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Bot");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

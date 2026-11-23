using LedgerBot.Bot.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LedgerBot.Bot.Data;

public sealed class ProcessedUpdateStore(BotDbContext dbContext, TimeProvider time)
{
    private const int DuplicateKeyNumber = 2627;
    private const int DuplicateUniqueIndexNumber = 2601;

    public async Task<bool> TryClaimAsync(long updateId, CancellationToken cancellationToken)
    {
        dbContext.ProcessedUpdates.Add(new(updateId, time.GetUtcNow()));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: DuplicateKeyNumber or DuplicateUniqueIndexNumber })
        {
            dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task MarkAsync(long updateId, ProcessedUpdateStatus status, CancellationToken cancellationToken)
    {
        await dbContext.ProcessedUpdates
            .Where(x => x.UpdateId == updateId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Status, status), cancellationToken);
    }
}

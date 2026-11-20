using Desfecho;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Data;
using LedgerBot.Ledger.Domain;
using Mediator;

namespace LedgerBot.Ledger.Features.CreateEntry;

public sealed class CreateEntryHandler(LedgerDbContext dbContext, TimeProvider time) : ICommandHandler<CreateEntryCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateEntryCommand command, CancellationToken cancellationToken)
    {
        var entry = new Entry(
            command.ChatId,
            command.Kind,
            command.Amount,
            command.OccurredOn,
            command.Description,
            command.CreatedBy,
            time.GetUtcNow());

        dbContext.Entries.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);

        return entry.Id;
    }
}

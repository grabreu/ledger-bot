using Desfecho;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LedgerBot.Ledger.Features.DeleteEntry;

public sealed class DeleteEntryHandler(LedgerDbContext dbContext) : ICommandHandler<DeleteEntryCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteEntryCommand command, CancellationToken cancellationToken)
    {
        var count = await dbContext.Entries
            .Where(x => x.Id == command.EntryId && x.ChatId == command.ChatId)
            .ExecuteDeleteAsync(cancellationToken);

        return count > 0
            ? Result.Success()
            : Result.NotFound("Entry not found.");
    }
}

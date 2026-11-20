using Desfecho;
using FluentValidation;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LedgerBot.Ledger.Features.ListEntries;

public sealed class ListEntriesHandler(LedgerDbContext dbContext) : IQueryHandler<ListEntriesQuery, Result<IReadOnlyList<EntryDto>>>
{
    public async ValueTask<Result<IReadOnlyList<EntryDto>>> Handle(ListEntriesQuery query, CancellationToken cancellationToken)
    {
        var entries = dbContext.Entries
            .AsNoTracking()
            .Where(x => x.ChatId == query.ChatId);

        if (query.From is { } from)
        {
            entries = entries.Where(x => x.OccurredOn >= from);
        }

        if (query.To is { } to)
        {
            entries = entries.Where(x => x.OccurredOn <= to);
        }

        var result = await entries
            .OrderByDescending(x => x.OccurredOn)
            .ThenByDescending(x => x.CreatedAt)
            .Take(query.Take)
            .Select(x => new EntryDto(x.Id, x.Kind, x.Amount, x.OccurredOn, x.Description))
            .ToListAsync(cancellationToken);

        return result;
    }
}

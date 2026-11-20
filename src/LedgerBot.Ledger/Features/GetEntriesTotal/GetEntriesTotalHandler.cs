using Desfecho;
using FluentValidation;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Data;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LedgerBot.Ledger.Features.GetEntriesTotal;

public sealed class GetEntriesTotalHandler(LedgerDbContext dbContext) : IQueryHandler<GetEntriesTotalQuery, Result<EntriesTotalDto>>
{
    public async ValueTask<Result<EntriesTotalDto>> Handle(GetEntriesTotalQuery query, CancellationToken cancellationToken)
    {
        var entries = dbContext.Entries
            .AsNoTracking()
            .Where(x => x.ChatId == query.ChatId && x.OccurredOn >= query.From && x.OccurredOn <= query.To);

        if (query.Kind is { } kind)
        {
            entries = entries.Where(x => x.Kind == kind);
        }

        if (!string.IsNullOrWhiteSpace(query.DescriptionContains))
        {
            entries = entries.Where(x => x.Description != null && x.Description.Contains(query.DescriptionContains));
        }

        var totals = await entries
            .GroupBy(x => x.Kind)
            .Select(group => new { Kind = group.Key, Amount = group.Sum(x => x.Amount), Count = group.Count() })
            .ToListAsync(cancellationToken);

        return new EntriesTotalDto(
            totals.Where(x => x.Kind == EntryKind.Income).Sum(x => x.Amount),
            totals.Where(x => x.Kind == EntryKind.Expense).Sum(x => x.Amount),
            totals.Sum(x => x.Count));
    }
}

using Desfecho;
using Mediator;

namespace LedgerBot.Ledger.Contracts.Queries;

public sealed record EntriesTotalDto(decimal Income, decimal Expense, int Count);

public sealed record GetEntriesTotalQuery(
    long ChatId,
    DateOnly From,
    DateOnly To,
    EntryKind? Kind,
    string? DescriptionContains) : IQuery<Result<EntriesTotalDto>>;


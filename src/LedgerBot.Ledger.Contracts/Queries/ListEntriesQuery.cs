using Desfecho;
using Mediator;

namespace LedgerBot.Ledger.Contracts.Queries;

public sealed record EntryDto(
    Guid Id,
    EntryKind Kind,
    decimal Amount,
    DateOnly OccurredOn,
    string? Description);

public sealed record ListEntriesQuery(
    long ChatId,
    DateOnly? From,
    DateOnly? To,
    int Take) : IQuery<Result<IReadOnlyList<EntryDto>>>;

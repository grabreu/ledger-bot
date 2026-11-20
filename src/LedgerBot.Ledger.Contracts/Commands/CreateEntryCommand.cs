using Desfecho;
using Mediator;

namespace LedgerBot.Ledger.Contracts.Commands;

public sealed record CreateEntryCommand(
    long ChatId,
    EntryKind Kind,
    decimal Amount,
    DateOnly OccurredOn,
    string? Description,
    long CreatedBy) : ICommand<Result<Guid>>;

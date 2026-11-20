using Desfecho;
using Mediator;

namespace LedgerBot.Ledger.Contracts.Commands;

public sealed record DeleteEntryCommand(
    long ChatId,
    Guid EntryId) : ICommand<Result>;

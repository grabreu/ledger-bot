using FluentValidation;
using LedgerBot.Ledger.Contracts.Commands;

namespace LedgerBot.Ledger.Features.DeleteEntry;

public sealed class DeleteEntryValidator : AbstractValidator<DeleteEntryCommand>
{
    public DeleteEntryValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.EntryId).NotEmpty();
    }
}

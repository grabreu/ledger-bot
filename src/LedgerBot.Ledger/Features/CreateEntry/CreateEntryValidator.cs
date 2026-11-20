using FluentValidation;
using LedgerBot.Ledger.Contracts.Commands;

namespace LedgerBot.Ledger.Features.CreateEntry;

public sealed class CreateEntryValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.CreatedBy).GreaterThan(0);
    }
}

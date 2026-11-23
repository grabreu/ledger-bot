using FluentValidation;
using LedgerBot.Ledger.Contracts.Commands;

namespace LedgerBot.Ledger.Features.CreateEntry;

public sealed class CreateEntryValidator : AbstractValidator<CreateEntryCommand>
{
    public CreateEntryValidator(TimeProvider time)
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, true);
        RuleFor(x => x.OccurredOn)
            .Must(date => date <= DateOnly.FromDateTime(time.GetLocalNow().DateTime))
            .WithMessage("The date cannot be in the future.");
        RuleFor(x => x.Description).MaximumLength(200);
        RuleFor(x => x.CreatedBy).GreaterThan(0);
    }
}

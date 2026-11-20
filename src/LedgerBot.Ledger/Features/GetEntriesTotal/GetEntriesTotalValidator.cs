using FluentValidation;
using LedgerBot.Ledger.Contracts.Queries;

namespace LedgerBot.Ledger.Features.GetEntriesTotal;

public sealed class GetEntriesTotalValidator : AbstractValidator<GetEntriesTotalQuery>
{
    public GetEntriesTotalValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.DescriptionContains).MaximumLength(200);
    }
}

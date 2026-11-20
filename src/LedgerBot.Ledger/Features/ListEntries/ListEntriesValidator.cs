using FluentValidation;
using LedgerBot.Ledger.Contracts.Queries;

namespace LedgerBot.Ledger.Features.ListEntries;

public sealed class ListEntriesValidator : AbstractValidator<ListEntriesQuery>
{
    public ListEntriesValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty();
        RuleFor(x => x.Take).InclusiveBetween(1, 20);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}

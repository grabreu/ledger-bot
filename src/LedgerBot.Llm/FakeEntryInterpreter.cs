using System.Globalization;
using System.Text.RegularExpressions;
using LedgerBot.Llm.Contracts;

namespace LedgerBot.Llm;

// Provisional stand-in for a real provider: takes the first number as the amount and the whole text as the description.
public sealed partial class FakeEntryInterpreter : IEntryInterpreter
{
    public Task<InterpretedEntry> InterpretAsync(string text, DateOnly today, CancellationToken cancellationToken = default)
    {
        decimal? amount = null;
        var match = AmountPattern().Match(text);
        if (match.Success)
        {
            amount = decimal.Parse(match.Value.Replace(',', '.'), CultureInfo.InvariantCulture);
        }

        return Task.FromResult(new InterpretedEntry(amount, Category: null, today, text.Trim()));
    }

    [GeneratedRegex(@"(?<!\d)\d+(?:[.,]\d{1,2})?(?!\d)")]
    private static partial Regex AmountPattern();
}

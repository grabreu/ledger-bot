using LedgerBot.Llm.Contracts;

namespace LedgerBot.Llm;

public sealed class FakeLlmProvider(TimeProvider timeProvider) : ILlmProvider
{
    private static readonly TimeZoneInfo s_timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public Task<InterpretedLedgerEntry?> InterpretAsync(string message, CancellationToken cancellationToken)
    {
        var words = message.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return Task.FromResult<InterpretedLedgerEntry?>(null);
        }

        var kind = words[0] switch
        {
            "gastei" => InterpretedLedgerEntryKind.Expense,
            "recebi" => InterpretedLedgerEntryKind.Income,
            _ => (InterpretedLedgerEntryKind?)null
        };

        if (kind is null)
        {
            return Task.FromResult<InterpretedLedgerEntry?>(null);
        }

        decimal? amount = words.Length > 1 && decimal.TryParse(words[1], out var v) ? v : null;
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), s_timeZone).DateTime);

        return Task.FromResult<InterpretedLedgerEntry?>(new InterpretedLedgerEntry(kind.Value, amount, today, null));
    }
}

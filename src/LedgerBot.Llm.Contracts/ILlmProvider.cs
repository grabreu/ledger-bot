namespace LedgerBot.Llm.Contracts;

public enum InterpretedLedgerEntryKind { Income, Expense }

public sealed record InterpretedLedgerEntry(InterpretedLedgerEntryKind Kind, decimal? Amount, DateOnly OccurredOn, string? Description);

public interface ILlmProvider
{
    Task<InterpretedLedgerEntry?> InterpretAsync(string message, CancellationToken cancellationToken);
}

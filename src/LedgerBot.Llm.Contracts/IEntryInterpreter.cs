namespace LedgerBot.Llm.Contracts;

public interface IEntryInterpreter
{
    // Interprets only: the result is a suggestion that code must validate before saving.
    Task<InterpretedEntry> InterpretAsync(string text, DateOnly today, CancellationToken cancellationToken = default);
}

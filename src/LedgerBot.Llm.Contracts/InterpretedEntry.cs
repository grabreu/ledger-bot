namespace LedgerBot.Llm.Contracts;

// A field is null when the message did not provide it.
public sealed record InterpretedEntry(decimal? Amount, string? Category, DateOnly? Date, string? Description);

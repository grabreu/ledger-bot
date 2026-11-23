namespace LedgerBot.Llm.Contracts;

public abstract record ToolEvent;

public sealed record EntryCreated(Guid EntryId) : ToolEvent;

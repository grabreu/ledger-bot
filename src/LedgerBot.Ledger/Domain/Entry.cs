using LedgerBot.Ledger.Contracts;

namespace LedgerBot.Ledger.Domain;

public sealed class Entry
{
    private Entry()
    {
    }

    public Entry(long chatId, EntryKind kind, decimal amount, DateOnly occurredOn, string? description, long createdBy, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        ChatId = chatId;
        Kind = kind;
        Amount = amount;
        OccurredOn = occurredOn;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public long ChatId { get; private set; }
    public EntryKind Kind { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly OccurredOn { get; private set; }
    public string? Description { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}

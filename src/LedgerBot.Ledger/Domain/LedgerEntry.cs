namespace LedgerBot.Ledger.Domain;

public sealed class LedgerEntry
{
    private LedgerEntry()
    {
    }

    public LedgerEntry(Guid householdId, LedgerEntryKind kind, decimal amount, DateOnly occurredOn, string? description, long createdBy, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        HouseholdId = householdId;
        Kind = kind;
        Amount = amount;
        OccurredOn = occurredOn;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid HouseholdId { get; private set; }
    public LedgerEntryKind Kind { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly OccurredOn { get; private set; }
    public string? Description { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}

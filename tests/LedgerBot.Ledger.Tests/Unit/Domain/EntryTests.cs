using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Domain;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Unit.Domain;

public sealed class EntryTests
{
    private readonly long _validChatId = 1;
    private readonly EntryKind _validKind = EntryKind.Expense;
    private readonly decimal _validAmount = 50m;
    private readonly DateOnly _validOccurredOn = new(2026, 11, 19);
    private readonly string _validDescription = "mercado";
    private readonly long _validCreatedBy = 1;
    private readonly DateTimeOffset _validCreatedAt = new(2026, 11, 19, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_ShouldSetProperties_WhenDataIsValid()
    {
        // Act
        var entry = new Entry(_validChatId, _validKind, _validAmount, _validOccurredOn, _validDescription, _validCreatedBy, _validCreatedAt);

        // Assert
        entry.Id.ShouldNotBe(Guid.Empty);
        entry.ChatId.ShouldBe(_validChatId);
        entry.Kind.ShouldBe(_validKind);
        entry.Amount.ShouldBe(_validAmount);
        entry.OccurredOn.ShouldBe(_validOccurredOn);
        entry.Description.ShouldBe(_validDescription);
        entry.CreatedBy.ShouldBe(_validCreatedBy);
        entry.CreatedAt.ShouldBe(_validCreatedAt);
    }
}

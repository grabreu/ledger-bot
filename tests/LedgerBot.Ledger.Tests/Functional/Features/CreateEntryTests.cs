using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Domain;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Functional.Features;

public sealed class CreateEntryTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    [Fact]
    public async Task Send_ShouldSaveEntry_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateEntryCommand(NewChatId(), EntryKind.Expense, 50m, new(2026, 11, 19), "mercado", 42);

        // Act
        var result = await SendAsync(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var savedEntry = await FindAsync<Entry>(result.Value);
        savedEntry.ShouldNotBeNull();
        savedEntry.ChatId.ShouldBe(command.ChatId);
        savedEntry.Kind.ShouldBe(command.Kind);
        savedEntry.Amount.ShouldBe(command.Amount);
        savedEntry.OccurredOn.ShouldBe(command.OccurredOn);
        savedEntry.Description.ShouldBe(command.Description);
        savedEntry.CreatedBy.ShouldBe(command.CreatedBy);
        savedEntry.CreatedAt.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Send_ShouldReturnErrorAndSaveNothing_WhenCommandIsInvalid()
    {
        // Arrange
        var countBefore = await CountAsync<Entry>();
        var command = new CreateEntryCommand(0, EntryKind.Expense, 50m, new(2026, 11, 19), "mercado", 42);

        // Act
        var result = await SendAsync(command);

        // Assert
        result.IsError.ShouldBeTrue();
        (await CountAsync<Entry>()).ShouldBe(countBefore);
    }
}

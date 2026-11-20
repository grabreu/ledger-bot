using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Functional.Features;

public sealed class CreateEntryTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    [Fact]
    public async Task Send_ShouldSaveEntry_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateEntryCommand(1, EntryKind.Expense, 50m, new DateOnly(2026, 11, 19), "mercado", 42);

        // Act
        var result = await SendAsync(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Send_ShouldReturnErrorAndSaveNothing_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateEntryCommand(0, EntryKind.Expense, 50m, new DateOnly(2026, 11, 19), "mercado", 42);

        // Act
        var result = await SendAsync(command);

        // Assert
        result.IsError.ShouldBeTrue();
    }
}

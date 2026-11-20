using FluentValidation.TestHelper;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Features.CreateEntry;

namespace LedgerBot.Ledger.Tests.Unit.Features.CreateEntry;

public sealed class CreateEntryValidatorTests
{
    private readonly CreateEntryValidator _validator = new();
    private readonly CreateEntryCommand _validCommand = new(1, EntryKind.Expense, 50m, new DateOnly(2026, 11, 19), "mercado", 42);

    [Fact]
    public void Validate_ShouldHaveNoErrors_WhenCommandIsValid()
    {
        // Act
        var result = _validator.TestValidate(_validCommand);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenChatIdIsZero()
    {
        // Arrange
        var command = _validCommand with { ChatId = 0 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId);
    }
}

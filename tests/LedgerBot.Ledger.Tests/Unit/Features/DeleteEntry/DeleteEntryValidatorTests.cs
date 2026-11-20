using FluentValidation.TestHelper;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Features.DeleteEntry;

namespace LedgerBot.Ledger.Tests.Unit.Features.DeleteEntry;

public sealed class DeleteEntryValidatorTests
{
    private readonly DeleteEntryValidator _validator = new();
    private readonly DeleteEntryCommand _validCommand = new(1, Guid.NewGuid());

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

    [Fact]
    public void Validate_ShouldHaveError_WhenEntryIdIsEmpty()
    {
        // Arrange
        var command = _validCommand with { EntryId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EntryId);
    }
}

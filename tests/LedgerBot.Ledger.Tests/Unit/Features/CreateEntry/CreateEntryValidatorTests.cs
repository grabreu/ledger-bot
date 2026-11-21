using FluentValidation.TestHelper;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Features.CreateEntry;

namespace LedgerBot.Ledger.Tests.Unit.Features.CreateEntry;

public sealed class CreateEntryValidatorTests
{
    private readonly CreateEntryValidator _validator = new();
    private readonly CreateEntryCommand _validCommand = new(1, EntryKind.Expense, 50m, new(2026, 11, 19), "mercado", 42);

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
    public void Validate_ShouldHaveError_WhenKindIsNotDefined()
    {
        // Arrange
        var command = _validCommand with { Kind = (EntryKind)99 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Kind);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenAmountIsNotPositive(decimal amount)
    {
        // Arrange
        var command = _validCommand with { Amount = amount };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    public static TheoryData<decimal> AmountsThatDoNotFitTheColumn => [10.999m, 10000000000000000m];

    [Theory]
    [MemberData(nameof(AmountsThatDoNotFitTheColumn))]
    public void Validate_ShouldHaveError_WhenAmountDoesNotFitTheColumn(decimal amount)
    {
        // Arrange
        var command = _validCommand with { Amount = amount };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    public static TheoryData<decimal> AmountsThatFitTheColumn => [0.01m, 45.90m, 45.900m, 9999999999999999.99m];

    [Theory]
    [MemberData(nameof(AmountsThatFitTheColumn))]
    public void Validate_ShouldNotHaveError_WhenAmountFitsTheColumn(decimal amount)
    {
        // Arrange
        var command = _validCommand with { Amount = amount };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionHasMaximumLength()
    {
        // Arrange
        var command = _validCommand with { Description = new('a', 200) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaximumLength()
    {
        // Arrange
        var command = _validCommand with { Description = new('a', 201) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenCreatedByIsNotPositive(long createdBy)
    {
        // Arrange
        var command = _validCommand with { CreatedBy = createdBy };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }
}

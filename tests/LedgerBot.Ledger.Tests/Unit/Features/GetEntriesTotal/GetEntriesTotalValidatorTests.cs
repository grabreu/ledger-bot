using FluentValidation.TestHelper;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Features.GetEntriesTotal;

namespace LedgerBot.Ledger.Tests.Unit.Features.GetEntriesTotal;

public sealed class GetEntriesTotalValidatorTests
{
    private readonly GetEntriesTotalValidator _validator = new();
    private readonly GetEntriesTotalQuery _validQuery = new(1, new(2026, 11, 1), new(2026, 11, 30), EntryKind.Expense, "mercado");

    [Fact]
    public void Validate_ShouldHaveNoErrors_WhenQueryIsValid()
    {
        // Act
        var result = _validator.TestValidate(_validQuery);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenChatIdIsZero()
    {
        // Arrange
        var query = _validQuery with { ChatId = 0 };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChatId);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenToIsBeforeFrom()
    {
        // Arrange
        var query = _validQuery with { From = new(2026, 11, 30), To = new(2026, 11, 1) };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenToEqualsFrom()
    {
        // Arrange
        var query = _validQuery with { From = new(2026, 11, 19), To = new(2026, 11, 19) };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenKindIsNotDefined()
    {
        // Arrange
        var query = _validQuery with { Kind = (EntryKind)99 };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Kind);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenKindIsNull()
    {
        // Arrange
        var query = _validQuery with { Kind = null };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Kind);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDescriptionContainsHasMaximumLength()
    {
        // Arrange
        var query = _validQuery with { DescriptionContains = new('a', 200) };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DescriptionContains);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionContainsExceedsMaximumLength()
    {
        // Arrange
        var query = _validQuery with { DescriptionContains = new('a', 201) };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DescriptionContains);
    }
}

using FluentValidation.TestHelper;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Features.ListEntries;

namespace LedgerBot.Ledger.Tests.Unit.Features.ListEntries;

public sealed class ListEntriesValidatorTests
{
    private readonly ListEntriesValidator _validator = new();
    private readonly ListEntriesQuery _validQuery = new(1, new(2026, 11, 1), new(2026, 11, 30), 10);

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

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validate_ShouldHaveError_WhenTakeIsOutOfRange(int take)
    {
        // Arrange
        var query = _validQuery with { Take = take };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Take);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    public void Validate_ShouldNotHaveError_WhenTakeIsOnTheLimit(int take)
    {
        // Arrange
        var query = _validQuery with { Take = take };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Take);
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
    public void Validate_ShouldNotHaveError_WhenOnlyToIsGiven()
    {
        // Arrange
        var query = _validQuery with { From = null };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.To);
    }
}

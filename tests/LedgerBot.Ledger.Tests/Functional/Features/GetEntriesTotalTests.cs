using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Domain;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Functional.Features;

public sealed class GetEntriesTotalTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    private readonly DateOnly _from = new(2026, 11, 1);
    private readonly DateOnly _to = new(2026, 11, 30);

    [Fact]
    public async Task Send_ShouldReturnTotals_WhenEntriesAreInPeriod()
    {
        // Arrange
        var chatId = NewChatId();
        await AddAsync(NewEntry(chatId, EntryKind.Income, 1000m, _from));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 50m, _to));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 25.5m, new(2026, 11, 15)));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(chatId, _from, _to, null, null));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Income.ShouldBe(1000m);
        result.Value.Expense.ShouldBe(75.5m);
        result.Value.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Send_ShouldIgnoreEntriesOutsideThePeriod()
    {
        // Arrange
        var chatId = NewChatId();
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 10m, _from.AddDays(-1)));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 20m, _to.AddDays(1)));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(chatId, _from, _to, null, null));

        // Assert
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Send_ShouldFilterByKind_WhenKindIsGiven()
    {
        // Arrange
        var chatId = NewChatId();
        await AddAsync(NewEntry(chatId, EntryKind.Income, 1000m, _from));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 50m, _from));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(chatId, _from, _to, EntryKind.Expense, null));

        // Assert
        result.Value.Income.ShouldBe(0m);
        result.Value.Expense.ShouldBe(50m);
        result.Value.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Send_ShouldFilterByDescription_WhenDescriptionContainsIsGiven()
    {
        // Arrange
        var chatId = NewChatId();
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 50m, _from, "mercado"));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 30m, _from, "farmacia"));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 20m, _from, null));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(chatId, _from, _to, null, "merc"));

        // Assert
        result.Value.Expense.ShouldBe(50m);
        result.Value.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Send_ShouldNotFilterByDescription_WhenDescriptionContainsIsBlank()
    {
        // Arrange
        var chatId = NewChatId();
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 50m, _from, "mercado"));
        await AddAsync(NewEntry(chatId, EntryKind.Expense, 30m, _from, "farmacia"));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(chatId, _from, _to, null, "  "));

        // Assert
        result.Value.Expense.ShouldBe(80m);
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Send_ShouldIgnoreEntriesOfAnotherChat()
    {
        // Arrange
        await AddAsync(NewEntry(NewChatId(), EntryKind.Expense, 50m, _from));

        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(NewChatId(), _from, _to, null, null));

        // Assert
        result.Value.Income.ShouldBe(0m);
        result.Value.Expense.ShouldBe(0m);
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Send_ShouldReturnError_WhenQueryIsInvalid()
    {
        // Act
        var result = await SendAsync(new GetEntriesTotalQuery(NewChatId(), _to, _from, null, null));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    private static Entry NewEntry(long chatId, EntryKind kind, decimal amount, DateOnly occurredOn, string? description = "mercado") =>
        new(chatId, kind, amount, occurredOn, description, 42, DateTimeOffset.UtcNow);
}

using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Ledger.Domain;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Functional.Features;

public sealed class ListEntriesTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    private readonly DateOnly _from = new(2026, 11, 1);
    private readonly DateOnly _to = new(2026, 11, 30);

    [Fact]
    public async Task Send_ShouldReturnEntriesOrderedByMostRecentDate()
    {
        // Arrange
        var chatId = NewChatId();
        var oldest = NewEntry(chatId, new(2026, 11, 10));
        var newest = NewEntry(chatId, new(2026, 11, 20), EntryKind.Income, 123.45m, "salario");
        var middle = NewEntry(chatId, new(2026, 11, 15));
        await AddAsync(oldest);
        await AddAsync(newest);
        await AddAsync(middle);

        // Act
        var result = await SendAsync(new ListEntriesQuery(chatId, null, null, 10));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Select(x => x.Id).ShouldBe([newest.Id, middle.Id, oldest.Id]);

        var first = result.Value[0];
        first.Kind.ShouldBe(newest.Kind);
        first.Amount.ShouldBe(newest.Amount);
        first.OccurredOn.ShouldBe(newest.OccurredOn);
        first.Description.ShouldBe(newest.Description);
    }

    [Fact]
    public async Task Send_ShouldReturnMostRecentlyCreatedFirst_WhenEntriesHaveTheSameDate()
    {
        // Arrange
        var chatId = NewChatId();
        var date = new DateOnly(2026, 11, 20);
        var createdLater = NewEntry(chatId, date, createdAt: DateTimeOffset.UtcNow);
        var createdEarlier = NewEntry(chatId, date, createdAt: DateTimeOffset.UtcNow.AddMinutes(-10));
        await AddAsync(createdLater);
        await AddAsync(createdEarlier);

        // Act
        var result = await SendAsync(new ListEntriesQuery(chatId, null, null, 10));

        // Assert
        result.Value.Select(x => x.Id).ShouldBe([createdLater.Id, createdEarlier.Id]);
    }

    [Fact]
    public async Task Send_ShouldReturnOnlyTheMostRecentEntries_WhenThereAreMoreThanTake()
    {
        // Arrange
        var chatId = NewChatId();
        var oldest = NewEntry(chatId, new(2026, 11, 10));
        var newest = NewEntry(chatId, new(2026, 11, 20));
        var middle = NewEntry(chatId, new(2026, 11, 15));
        await AddAsync(oldest);
        await AddAsync(newest);
        await AddAsync(middle);

        // Act
        var result = await SendAsync(new ListEntriesQuery(chatId, null, null, 2));

        // Assert
        result.Value.Select(x => x.Id).ShouldBe([newest.Id, middle.Id]);
    }

    [Fact]
    public async Task Send_ShouldReturnOnlyEntriesInPeriod_WhenFromAndToAreGiven()
    {
        // Arrange
        var chatId = NewChatId();
        var onFrom = NewEntry(chatId, _from);
        var onTo = NewEntry(chatId, _to);
        await AddAsync(NewEntry(chatId, _from.AddDays(-1)));
        await AddAsync(onFrom);
        await AddAsync(onTo);
        await AddAsync(NewEntry(chatId, _to.AddDays(1)));

        // Act
        var result = await SendAsync(new ListEntriesQuery(chatId, _from, _to, 10));

        // Assert
        result.Value.Select(x => x.Id).ShouldBe([onTo.Id, onFrom.Id]);
    }

    [Fact]
    public async Task Send_ShouldIgnoreEntriesOfAnotherChat()
    {
        // Arrange
        await AddAsync(NewEntry(NewChatId(), _from));

        // Act
        var result = await SendAsync(new ListEntriesQuery(NewChatId(), null, null, 10));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Send_ShouldReturnError_WhenQueryIsInvalid()
    {
        // Act
        var result = await SendAsync(new ListEntriesQuery(NewChatId(), null, null, 0));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    private static Entry NewEntry(
        long chatId,
        DateOnly occurredOn,
        EntryKind kind = EntryKind.Expense,
        decimal amount = 50m,
        string? description = "mercado",
        DateTimeOffset? createdAt = null) =>
        new(chatId, kind, amount, occurredOn, description, 42, createdAt ?? DateTimeOffset.UtcNow);
}

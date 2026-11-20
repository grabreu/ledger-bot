using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Domain;
using Shouldly;

namespace LedgerBot.Ledger.Tests.Functional.Features;

public sealed class DeleteEntryTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    [Fact]
    public async Task Send_ShouldDeleteEntry_WhenEntryBelongsToChat()
    {
        // Arrange
        var entry = NewEntry(NewChatId());
        await AddAsync(entry);

        // Act
        var result = await SendAsync(new DeleteEntryCommand(entry.ChatId, entry.Id));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        (await FindAsync<Entry>(entry.Id)).ShouldBeNull();
    }

    [Fact]
    public async Task Send_ShouldReturnErrorAndKeepEntry_WhenEntryBelongsToAnotherChat()
    {
        // Arrange
        var entry = NewEntry(NewChatId());
        await AddAsync(entry);

        // Act
        var result = await SendAsync(new DeleteEntryCommand(NewChatId(), entry.Id));

        // Assert
        result.IsError.ShouldBeTrue();
        (await FindAsync<Entry>(entry.Id)).ShouldNotBeNull();
    }

    [Fact]
    public async Task Send_ShouldReturnError_WhenEntryDoesNotExist()
    {
        // Act
        var result = await SendAsync(new DeleteEntryCommand(NewChatId(), Guid.NewGuid()));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    private static Entry NewEntry(long chatId) =>
        new(chatId, EntryKind.Expense, 50m, new(2026, 11, 19), "mercado", 42, DateTimeOffset.UtcNow);
}

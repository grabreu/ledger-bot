using LedgerBot.Bot.Domain;
using Shouldly;

namespace LedgerBot.Bot.Tests.Functional;

public sealed class ProcessedUpdateStoreTests(FunctionalTestFixture fixture) : FunctionalTestBase(fixture)
{
    [Fact]
    public async Task TryClaimAsync_ShouldClaim_WhenUpdateIsNew()
    {
        // Arrange
        var updateId = NewUpdateId();

        // Act
        var claimed = await WithStoreAsync(store => store.TryClaimAsync(updateId, TestContext.Current.CancellationToken));

        // Assert
        claimed.ShouldBeTrue();
        var row = await FindAsync(updateId);
        row.ShouldNotBeNull();
        row.Status.ShouldBe(ProcessedUpdateStatus.Processing);
    }

    [Fact]
    public async Task TryClaimAsync_ShouldNotClaim_WhenUpdateWasAlreadyClaimed()
    {
        // Arrange
        var updateId = NewUpdateId();
        await WithStoreAsync(store => store.TryClaimAsync(updateId, TestContext.Current.CancellationToken));

        // Act
        var claimed = await WithStoreAsync(store => store.TryClaimAsync(updateId, TestContext.Current.CancellationToken));

        // Assert
        claimed.ShouldBeFalse();
    }

    [Fact]
    public async Task TryClaimAsync_ShouldClaimOnlyOnce_WhenCalledConcurrently()
    {
        // Arrange
        var updateId = NewUpdateId();

        // Act
        var results = await Task.WhenAll(Enumerable.Range(0, 8)
            .Select(_ => WithStoreAsync(store => store.TryClaimAsync(updateId, TestContext.Current.CancellationToken))));

        // Assert
        results.Count(claimed => claimed).ShouldBe(1);
    }

    [Theory]
    [InlineData(ProcessedUpdateStatus.Completed)]
    [InlineData(ProcessedUpdateStatus.Failed)]
    public async Task MarkAsync_ShouldSetTheStatus_WhenUpdateWasClaimed(ProcessedUpdateStatus status)
    {
        // Arrange
        var updateId = NewUpdateId();
        await WithStoreAsync(store => store.TryClaimAsync(updateId, TestContext.Current.CancellationToken));

        // Act
        await WithStoreAsync(async store =>
        {
            await store.MarkAsync(updateId, status, TestContext.Current.CancellationToken);
            return true;
        });

        // Assert
        var row = await FindAsync(updateId);
        row.ShouldNotBeNull();
        row.Status.ShouldBe(status);
    }
}

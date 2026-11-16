namespace LedgerBot.Llm.Tests.Unit;

public class FakeEntryInterpreterTests
{
    private static readonly DateOnly s_today = new(2026, 11, 16);
    private readonly FakeEntryInterpreter _interpreter = new();

    [Theory]
    [InlineData("gastei 50 no mercado", "50")]
    [InlineData("mercado 50,90", "50.90")]
    [InlineData("uber 12.5 ontem", "12.5")]
    public async Task InterpretAsync_ShouldReadAmount_WhenTextHasNumber(string text, string expected)
    {
        var entry = await _interpreter.InterpretAsync(text, s_today, TestContext.Current.CancellationToken);

        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture), entry.Amount);
    }

    [Fact]
    public async Task InterpretAsync_ShouldLeaveAmountEmpty_WhenTextHasNoNumber()
    {
        var entry = await _interpreter.InterpretAsync("comprei pão", s_today, TestContext.Current.CancellationToken);

        Assert.Null(entry.Amount);
    }

    [Fact]
    public async Task InterpretAsync_ShouldUseToday_AndLeaveCategoryEmpty()
    {
        var entry = await _interpreter.InterpretAsync("gastei 50 no mercado", s_today, TestContext.Current.CancellationToken);

        Assert.Equal(s_today, entry.Date);
        Assert.Null(entry.Category);
        Assert.Equal("gastei 50 no mercado", entry.Description);
    }
}

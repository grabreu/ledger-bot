namespace LedgerBot.Ledger.Tests.Unit;

public sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    private static readonly TimeZoneInfo s_zone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public override DateTimeOffset GetUtcNow() => now;

    public override TimeZoneInfo LocalTimeZone => s_zone;
}

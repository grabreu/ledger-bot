namespace LedgerBot.Api;

internal sealed class SaoPauloTimeProvider : TimeProvider
{
    private static readonly TimeZoneInfo s_zone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public override TimeZoneInfo LocalTimeZone => s_zone;
}

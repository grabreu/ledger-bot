namespace LedgerBot.Llm.Tools;

internal static class LocalTime
{
    private static readonly TimeZoneInfo s_brt = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public static DateOnly Today(TimeProvider time) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(time.GetUtcNow(), s_brt).DateTime);
}

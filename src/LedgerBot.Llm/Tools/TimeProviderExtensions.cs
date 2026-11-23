namespace LedgerBot.Llm.Tools;

internal static class TimeProviderExtensions
{
    public static DateOnly Today(this TimeProvider time) => DateOnly.FromDateTime(time.GetLocalNow().DateTime);
}

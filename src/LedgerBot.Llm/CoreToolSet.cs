using LedgerBot.Llm.Contracts;

namespace LedgerBot.Llm;

public sealed class CoreToolSet(TimeProvider time) : ILlmToolSet
{
    private static readonly TimeZoneInfo s_brt = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public IEnumerable<AITool> GetTools()
    {
        yield return AIFunctionFactory.Create(GetCurrentDate, name: "core_get_current_date");
    }

    [Description("Retorna a data atual do usuário (yyyy-MM-dd).")]
    private string GetCurrentDate() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(time.GetUtcNow(), s_brt).DateTime).ToString("yyyy-MM-dd");
}

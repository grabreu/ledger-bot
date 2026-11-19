using System.ComponentModel;
using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm;

public sealed class CoreToolSet(TimeProvider time) : ILlmToolSet
{
    private static readonly TimeZoneInfo s_brt = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public IEnumerable<AITool> GetTools()
    {
        yield return AIFunctionFactory.Create(GetCurrentDate, name: "core_get_current_date");
    }

    [Description("Returns today's date for the user (America/Sao_Paulo) as yyyy-MM-dd. Call it whenever the answer depends on the current date, such as \"today\", \"yesterday\", \"this month\" or \"this week\". Never guess the date.")]
    private string GetCurrentDate() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(time.GetUtcNow(), s_brt).DateTime).ToString("yyyy-MM-dd");
}

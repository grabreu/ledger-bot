using System.ComponentModel;
using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm.Tools;

public sealed class CoreToolSet(TimeProvider time) : ILlmToolSet
{
    public IEnumerable<AITool> GetTools(UserContext user, ICollection<ToolEvent> events)
    {
        yield return AIFunctionFactory.Create(GetCurrentDate, name: "core_get_current_date");
    }

    [Description("Returns today's date for the user (America/Sao_Paulo) as yyyy-MM-dd. Call it whenever the answer depends on the current date, such as \"today\", \"yesterday\", \"this month\" or \"this week\". Never guess the date.")]
    private string GetCurrentDate() => LocalTime.Today(time).ToString("yyyy-MM-dd");
}

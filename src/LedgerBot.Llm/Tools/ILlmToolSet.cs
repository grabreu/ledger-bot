using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm.Tools;

public interface ILlmToolSet
{
    IEnumerable<AITool> GetTools(UserContext user, ICollection<ToolEvent> events);
}

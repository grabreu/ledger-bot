using Microsoft.Extensions.AI;

namespace LedgerBot.Llm.Contracts;

public interface ILlmToolSet
{
    IEnumerable<AITool> GetTools();
}

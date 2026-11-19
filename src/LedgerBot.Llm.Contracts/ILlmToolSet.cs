namespace LedgerBot.Llm.Contracts;

public interface ILlmToolSet
{
    IEnumerable<AITool> GetTools();
}

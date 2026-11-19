using LedgerBot.Llm.Contracts;

namespace LedgerBot.Llm;

public sealed class LlmService(IChatClient chatClient, IEnumerable<ILlmToolSet> toolSets) : ILlmService
{
    private const string SystemPrompt = """
        Você é o assistente do LedgerBot.
        Responda de forma objetiva e utilize as ferramentas disponíveis
        quando forem relevantes.
        Não invente dados financeiros.
        """;

    public async Task<LlmChatResponse> ProcessMessageAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, message)
        };

        var options = new ChatOptions
        {
            Tools = [.. toolSets.SelectMany(toolSet => toolSet.GetTools())]
        };

        var response = await chatClient.GetResponseAsync(messages, options, cancellationToken);

        return new LlmChatResponse(conversationId, response.Text);
    }
}

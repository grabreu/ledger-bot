namespace LedgerBot.Llm.Contracts;

public sealed record LlmChatResponse(Guid ConversationId, string Message);

public interface ILlmService
{
    Task<LlmChatResponse> ProcessMessageAsync(Guid conversationId, string message, CancellationToken cancellationToken = default);
}

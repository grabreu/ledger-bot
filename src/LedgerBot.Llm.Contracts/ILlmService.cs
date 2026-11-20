namespace LedgerBot.Llm.Contracts;

public sealed record UserContext(long ChatId, long TelegramUserId);

public sealed record LlmChatResponse(string Message);

public interface ILlmService
{
    Task<LlmChatResponse> ProcessMessageAsync(UserContext user, string message, CancellationToken cancellationToken = default);
}

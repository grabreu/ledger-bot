using LedgerBot.Llm.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramUpdateHandler(ITelegramBotClient bot, ILlmService llm)
{
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message is not { Text: { } text } message)
        {
            return;
        }

        // No conversation is persisted yet, so each message gets its own id.
        var response = await llm.ProcessMessageAsync(Guid.NewGuid(), text, cancellationToken);

        await bot.SendMessage(message.Chat.Id, response.Message, cancellationToken: cancellationToken);
    }
}

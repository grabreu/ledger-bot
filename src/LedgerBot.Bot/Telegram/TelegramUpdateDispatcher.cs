using LedgerBot.Llm.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramUpdateDispatcher(ITelegramBotClient bot, ILlmService llm)
{
    public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } text, From: { } from } message)
        {
            return;
        }

        var user = new UserContext(message.Chat.Id, from.Id);

        var response = await llm.ProcessMessageAsync(user, text, cancellationToken);

        await bot.SendMessage(message.Chat.Id, response.Message, cancellationToken: cancellationToken);
    }
}

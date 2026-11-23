using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramUpdateDispatcher(ITelegramBotClient bot)
{
    public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
    {
        switch (update)
        {
            case { Message: { Text: { } text, From: { } from } message }:
                await OnMessageAsync(message, from.Id, text, cancellationToken);
                break;
        }
    }

    private async Task OnMessageAsync(Message message, long userId, string text, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (IsStartCommand(message, text))
        {
            await bot.SendMessage(
                chatId,
                text: "Olá! Sou seu assistente financeiro.\n\nUse /help para ver os comandos disponíveis.",
                cancellationToken: cancellationToken);
            return;
        }

        await bot.SendMessage(chatId, text, cancellationToken: cancellationToken);
    }

    private static bool IsStartCommand(Message message, string text)
    {
        var command = message.Entities?.FirstOrDefault(e => e.Type == MessageEntityType.BotCommand && e.Offset == 0);
        return command is not null && text[..command.Length].Split('@')[0] == "/start";
    }
}

using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Llm.Contracts;
using Mediator;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LedgerBot.Bot.Telegram;

public sealed class TelegramUpdateDispatcher(ITelegramBotClient bot, ILlmService llm, ISender sender)
{
    private const string WelcomeText = "Oi! Eu anoto seus gastos e receitas e mostro seus totais. Experimente: \"gastei 45 no mercado\", \"recebi 3000 de salário\" ou \"quanto gastei em outubro?\".";

    public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
    {
        switch (update)
        {
            case { Message: { Text: { } text, From: { } from } message }:
                await OnMessageAsync(message, from.Id, text, cancellationToken);
                break;

            case { CallbackQuery: { Data: { } data, Message: { } origin } callback }:
                await OnCallbackQueryAsync(callback, data, origin, cancellationToken);
                break;
        }
    }

    private async Task OnMessageAsync(Message message, long userId, string text, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        if (IsStartCommand(message, text))
        {
            await bot.SendMessage(chatId, WelcomeText, cancellationToken: cancellationToken);
            return;
        }

        var response = await llm.ProcessMessageAsync(new UserContext(chatId, userId), text, cancellationToken);

        await bot.SendMessage(chatId, response.Message, replyMarkup: BuildButtons(response.Events), cancellationToken: cancellationToken);
    }

    private async Task OnCallbackQueryAsync(CallbackQuery callback, string data, Message origin, CancellationToken cancellationToken)
    {
        if (!CallbackData.TryParse(data, out var action, out var id))
        {
            return;
        }

        var chatId = origin.Chat.Id;

        var reply = action switch
        {
            CallbackAction.Undo => await UndoAsync(chatId, id, cancellationToken),
            _ => null
        };

        if (reply is null)
        {
            return;
        }

        await bot.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
        await bot.EditMessageText(chatId, origin.Id, reply.Text, replyMarkup: reply.Markup, cancellationToken: cancellationToken);
    }

    private async Task<CallbackReply> UndoAsync(long chatId, Guid entryId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteEntryCommand(chatId, entryId), cancellationToken);

        return new(result.IsSuccess ? "Desfeito." : "Essa entrada já foi desfeita.");
    }

    private static bool IsStartCommand(Message message, string text)
    {
        var command = message.Entities?.FirstOrDefault(e => e.Type == MessageEntityType.BotCommand && e.Offset == 0);

        return command is not null && text[..command.Length].Split('@')[0] == "/start";
    }

    private static InlineKeyboardMarkup? BuildButtons(IReadOnlyList<ToolEvent> events)
    {
        var buttons = events
            .Select(toolEvent => toolEvent switch
            {
                EntryCreated created => InlineKeyboardButton.WithCallbackData("Desfazer", CallbackData.Create(CallbackAction.Undo, created.EntryId)),
                _ => null
            })
            .OfType<InlineKeyboardButton>()
            .ToArray();

        return buttons.Length == 0 ? null : new InlineKeyboardMarkup(buttons);
    }

    private sealed record CallbackReply(string Text, InlineKeyboardMarkup? Markup = null);
}

using LedgerBot.Llm.Contracts;

namespace LedgerBot.Bot.Features.InterpretMessage;

// Provisional: replies with what was understood; nothing is saved yet.
public sealed class InterpretMessageHandler(ITelegramBotClient bot, ILlmProvider llm)
{
    private static readonly CultureInfo s_culture = CultureInfo.GetCultureInfo("pt-BR");

    public async Task HandleAsync(long chatId, string text, CancellationToken cancellationToken = default)
    {
        var entry = await llm.InterpretAsync(text, cancellationToken);

        var reply = entry is null
            ? "Não entendi a mensagem."
            : string.Join('\n',
                $"Tipo: {(entry.Kind == InterpretedLedgerEntryKind.Expense ? "Despesa" : "Receita")}",
                $"Valor: {entry.Amount?.ToString("N2", s_culture) ?? "?"}",
                $"Data: {entry.OccurredOn.ToString("d", s_culture)}",
                $"Descrição: {entry.Description ?? "?"}");

        await bot.SendMessage(chatId, reply, cancellationToken: cancellationToken);
    }
}

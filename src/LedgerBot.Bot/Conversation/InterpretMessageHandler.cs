using System.Globalization;
using LedgerBot.Bot.Channels;
using LedgerBot.Llm.Contracts;

namespace LedgerBot.Bot.Conversation;

// Provisional: replies with what was understood; nothing is saved yet.
public sealed class InterpretMessageHandler(
    IChannelSender channel,
    IEntryInterpreter interpreter,
    TimeProvider timeProvider) : IIncomingMessageHandler
{
    private static readonly CultureInfo s_culture = CultureInfo.GetCultureInfo("pt-BR");
    private static readonly TimeZoneInfo s_timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public async Task HandleAsync(IncomingMessage message, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), s_timeZone).DateTime);
        var entry = await interpreter.InterpretAsync(message.Text, today, cancellationToken);

        var reply = string.Join('\n',
            $"Valor: {entry.Amount?.ToString("N2", s_culture) ?? "?"}",
            $"Categoria: {entry.Category ?? "?"}",
            $"Data: {entry.Date?.ToString("d", s_culture) ?? "?"}",
            $"Descrição: {entry.Description ?? "?"}");

        await channel.SendTextAsync(message.ChatId, reply, cancellationToken);
    }
}

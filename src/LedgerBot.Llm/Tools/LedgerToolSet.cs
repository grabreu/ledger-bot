using System.ComponentModel;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Llm.Contracts;
using Mediator;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm.Tools;

public sealed class LedgerToolSet(ISender sender) : ILlmToolSet
{
    private const string CreateEntryDescription = "Records one income or expense the user reported, for example \"gastei 45 no mercado\" or \"recebi 3000 de salário\". Call it exactly once per message, only after the amount is known; if the amount is missing, ask the user instead of calling. Resolve relative dates (\"hoje\", \"ontem\") with core_get_current_date before calling. Never guess the amount. The result has Saved: tell the user it was saved only when Saved is true, otherwise use Errors to say what is missing.";

    public IEnumerable<AITool> GetTools(UserContext user)
    {
        yield return AIFunctionFactory.Create(
            (
                [Description("Expense for money spent (gastei, paguei, comprei); Income for money received (recebi, ganhei, salário)")] EntryKind kind,
                [Description("Amount in BRL as a positive number without currency symbol, for example 45.90")] decimal amount,
                [Description("Date the entry happened as yyyy-MM-dd; today's date when the user does not say otherwise")] DateOnly ocurredOn,
                [Description("Short label of what it was, taken from the user's words, for example \"mercado\"; no amounts or dates")] string description,
                CancellationToken cancellationToken) => CreateEntryAsync(user, kind, amount, ocurredOn, description, cancellationToken),
            new AIFunctionFactoryOptions { Name = "ledger_create_entry", Description = CreateEntryDescription });
    }

    private async Task<CreateEntryToolResult> CreateEntryAsync(
        UserContext user,
        EntryKind kind,
        decimal amount,
        DateOnly ocurredOn,
        string description,
        CancellationToken cancellationToken)
    {
        var command = new CreateEntryCommand(user.ChatId, kind, amount, ocurredOn, description, user.TelegramUserId);
        var result = await sender.Send(command, cancellationToken);
        return result.Match(
            value => new CreateEntryToolResult(Saved: true, value, []),
            errors => new CreateEntryToolResult(Saved: false, null, [.. errors.Select(e => $"{e.Property}: {e.Description}")]));
    }
}

public sealed record CreateEntryToolResult(bool Saved, Guid? EntryId, string[] Errors);

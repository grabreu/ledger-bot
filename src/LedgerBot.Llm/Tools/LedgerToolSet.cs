using System.ComponentModel;
using LedgerBot.Ledger.Contracts;
using LedgerBot.Ledger.Contracts.Commands;
using LedgerBot.Ledger.Contracts.Queries;
using LedgerBot.Llm.Contracts;
using Mediator;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm.Tools;

public sealed class LedgerToolSet(ISender sender) : ILlmToolSet
{
    private const string CreateEntryDescription = "Records one income or expense the user reported, for example \"gastei 45 no mercado\" or \"recebi 3000 de salário\". Call it exactly once per message, only after the amount is known; if the amount is missing, ask the user instead of calling. Resolve relative dates (\"hoje\", \"ontem\") with core_get_current_date before calling. Never guess the amount. The result has Saved: tell the user it was saved only when Saved is true, otherwise use Errors to say what is missing.";

    private const string GetEntriesTotalDescription = "Returns the total income, total expense and number of entries between two dates, for example to answer \"quanto gastei com mercado em outubro?\". Use it for any question about how much was spent or received; never add up entries yourself. Resolve relative dates (\"este mês\", \"semana passada\") with core_get_current_date, and use the current year when the user gives a month without a year. The result has Total (Income, Expense, Count): balance is Income minus Expense. When Errors is not empty, the query failed; say what is wrong instead of guessing.";

    private const string ListEntriesDescription = "Lists the most recent entries, newest first, optionally between two dates, for example to answer \"o que eu anotei hoje?\". Use it when the user asks to see entries, not to compute totals. Resolve relative dates with core_get_current_date. When Errors is not empty, the query failed; say what is wrong instead of guessing.";

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

        yield return AIFunctionFactory.Create(
            (
                [Description("Start date as yyyy-MM-dd")] DateOnly from,
                [Description("End date as yyyy-MM-dd, inclusive")] DateOnly to,
                [Description("Expense or Income; omit to include both")] EntryKind? kind = null,
                [Description("Text the entry description must contain, for example \"mercado\"; omit to include all entries")] string? descriptionContains = null,
                CancellationToken cancellationToken = default) => GetEntriesTotalAsync(user, from, to, kind, descriptionContains, cancellationToken),
            new AIFunctionFactoryOptions { Name = "ledger_get_entries_total", Description = GetEntriesTotalDescription });

        yield return AIFunctionFactory.Create(
            (
                [Description("Start date as yyyy-MM-dd; omit for no lower bound")] DateOnly? from = null,
                [Description("End date as yyyy-MM-dd, inclusive; omit for no upper bound")] DateOnly? to = null,
                [Description("How many entries to return, from 1 to 20")] int take = 10,
                CancellationToken cancellationToken = default) => ListEntriesAsync(user, from, to, take, cancellationToken),
            new AIFunctionFactoryOptions { Name = "ledger_list_entries", Description = ListEntriesDescription });
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

    private async Task<GetEntriesTotalToolResult> GetEntriesTotalAsync(
        UserContext user,
        DateOnly from,
        DateOnly to,
        EntryKind? kind,
        string? descriptionContains,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetEntriesTotalQuery(user.ChatId, from, to, kind, descriptionContains), cancellationToken);
        return result.Match(
            total => new GetEntriesTotalToolResult(total, []),
            errors => new GetEntriesTotalToolResult(null, [.. errors.Select(e => $"{e.Property}: {e.Description}")]));
    }

    private async Task<ListEntriesToolResult> ListEntriesAsync(
        UserContext user,
        DateOnly? from,
        DateOnly? to,
        int take,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListEntriesQuery(user.ChatId, from, to, take), cancellationToken);
        return result.Match(
            entries => new ListEntriesToolResult(entries, []),
            errors => new ListEntriesToolResult(null, [.. errors.Select(e => $"{e.Property}: {e.Description}")]));
    }
}

public sealed record CreateEntryToolResult(bool Saved, Guid? EntryId, string[] Errors);

public sealed record GetEntriesTotalToolResult(EntriesTotalDto? Total, string[] Errors);

public sealed record ListEntriesToolResult(IReadOnlyList<EntryDto>? Entries, string[] Errors);

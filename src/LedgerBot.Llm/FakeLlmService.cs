using System.Text.RegularExpressions;
using LedgerBot.Llm.Contracts;
using Microsoft.Extensions.AI;

namespace LedgerBot.Llm;

// Stand-in for the model: picks a tool by matching the message against fixed rules, runs it and formats the reply.
// It understands nothing beyond the rules below.
public sealed partial class FakeLlmService(IEnumerable<ILlmToolSet> toolSets) : ILlmService
{
    private static readonly Rule[] s_rules =
    [
        new(CurrentDatePattern(), "core_get_current_date", _ => [], result => $"Hoje é {result}.")
    ];

    public async Task<LlmChatResponse> ProcessMessageAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
    {
        var rule = s_rules.FirstOrDefault(x => x.Pattern.IsMatch(message));

        if (rule is null)
        {
            return new LlmChatResponse(conversationId, "Não entendi a mensagem.");
        }

        var tool = toolSets
            .SelectMany(toolSet => toolSet.GetTools())
            .OfType<AIFunction>()
            .Single(x => x.Name == rule.Tool);

        var result = await tool.InvokeAsync(rule.Arguments(rule.Pattern.Match(message)), cancellationToken);

        return new LlmChatResponse(conversationId, rule.Reply(result));
    }

    [GeneratedRegex(@"\b(data|dia|hoje)\b", RegexOptions.IgnoreCase)]
    private static partial Regex CurrentDatePattern();

    private sealed record Rule(Regex Pattern, string Tool, Func<Match, AIFunctionArguments> Arguments, Func<object?, string> Reply);
}

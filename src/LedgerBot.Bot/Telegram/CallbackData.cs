namespace LedgerBot.Bot.Telegram;

internal enum CallbackAction { Undo }

internal static class CallbackData
{
    private const char Separator = ':';

    public static string Create(CallbackAction action, Guid id) => $"{action.ToString().ToLowerInvariant()}{Separator}{id}";

    public static bool TryParse(string data, out CallbackAction action, out Guid id)
    {
        action = default;
        id = default;

        var separator = data.IndexOf(Separator);

        return separator > 0
            && Enum.TryParse(data.AsSpan(0, separator), ignoreCase: true, out action)
            && Enum.IsDefined(action)
            && Guid.TryParse(data.AsSpan(separator + 1), out id);
    }
}

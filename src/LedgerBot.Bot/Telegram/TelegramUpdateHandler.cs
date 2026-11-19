namespace LedgerBot.Bot.Telegram;

public sealed class TelegramUpdateHandler(ITelegramBotClient bot)
{
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message is not { Text: { } text } message)
        {
            return;
        }

        await bot.SendMessage(message.Chat.Id, text, cancellationToken: cancellationToken);
    }
}

using LedgerBot.Bot.Channels;
using LedgerBot.Bot.Conversation;
using LedgerBot.Llm;

namespace LedgerBot.Bot.Tests;

public class InterpretMessageHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldReplyWithInterpretedEntry_WhenMessageHasAmount()
    {
        // 02:00 UTC on 17/11 is still 16/11 in America/Sao_Paulo.
        var time = new FixedTimeProvider(new DateTimeOffset(2026, 11, 17, 2, 0, 0, TimeSpan.Zero));
        var sender = new RecordingSender();
        var handler = new InterpretMessageHandler(sender, new FakeEntryInterpreter(), time);
        var message = new IncomingMessage(new ChannelUser("telegram", "1"), "10", "gastei 50 no mercado");

        await handler.HandleAsync(message, TestContext.Current.CancellationToken);

        var (chatId, text) = Assert.Single(sender.Sent);
        Assert.Equal("10", chatId);
        Assert.Contains("Valor: 50,00", text);
        Assert.Contains("Categoria: ?", text);
        Assert.Contains("Data: 16/11/2026", text);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class RecordingSender : IChannelSender
    {
        public List<(string ChatId, string Text)> Sent { get; } = [];

        public Task SendTextAsync(string chatId, string text, CancellationToken cancellationToken = default)
        {
            Sent.Add((chatId, text));
            return Task.CompletedTask;
        }
    }
}

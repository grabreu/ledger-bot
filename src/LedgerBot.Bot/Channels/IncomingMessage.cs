namespace LedgerBot.Bot.Channels;

public sealed record IncomingMessage(ChannelUser User, string ChatId, string Text);

var builder = DistributedApplication.CreateBuilder(args);

var telegramBotToken = builder.AddParameter("telegram-bot-token", secret: true);

var telegramWebhookSecret = builder.AddParameter(
    "telegram-webhook-secret",
    new GenerateParameterDefault { MinLength = 32, Special = false },
    secret: true,
    persist: true);

var bot = builder.AddProject<Projects.LedgerBot_Bot>("bot");
var botHttps = bot.GetEndpoint("https");

var tunnel = builder.AddDevTunnel("bot-tunnel")
    .WithReference(botHttps, allowAnonymous: true);

bot.WithEnvironment("Telegram__BotToken", telegramBotToken)
    .WithEnvironment("Telegram__WebhookSecret", telegramWebhookSecret)
    .WithEnvironment("Telegram__WebhookUrl", tunnel.GetEndpoint(botHttps));

var app = builder.Build();

await app.RunAsync();

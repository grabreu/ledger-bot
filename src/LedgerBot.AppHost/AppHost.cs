var builder = DistributedApplication.CreateBuilder(args);

var telegramBotToken = builder.AddParameter("telegram-bot-token", secret: true);

var telegramWebhookSecret = builder.AddParameter(
    "telegram-webhook-secret",
    new GenerateParameterDefault { MinLength = 32, Special = false },
    secret: true,
    persist: true);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("ledgerbot");

var api = builder.AddProject<Projects.LedgerBot_Api>("api");

var apiHttps = api.GetEndpoint("https");

var tunnel = builder.AddDevTunnel("api-tunnel")
    .WithReference(apiHttps, allowAnonymous: true);

api.WithReference(db)
    .WithEnvironment("Telegram__BotToken", telegramBotToken)
    .WithEnvironment("Telegram__WebhookSecret", telegramWebhookSecret)
    .WithEnvironment("Telegram__WebhookUrl", tunnel.GetEndpoint(apiHttps))
    .WaitFor(db);

var app = builder.Build();

await app.RunAsync();

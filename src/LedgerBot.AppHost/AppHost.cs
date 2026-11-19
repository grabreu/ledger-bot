var builder = DistributedApplication.CreateBuilder(args);

var telegramBotToken = builder.AddParameter("telegram-bot-token", secret: true);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("ledgerbot");

builder.AddDotnetProject("api", "../LedgerBot.Api/LedgerBot.Api.csproj")
    .WithReference(db)
    .WithEnvironment("Telegram__BotToken", telegramBotToken)
    .WaitFor(db);

var app = builder.Build();

await app.RunAsync();

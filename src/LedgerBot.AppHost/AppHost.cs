var builder = DistributedApplication.CreateBuilder(args);

var telegramBotToken = builder.AddParameter("telegram-bot-token", secret: true);

builder.AddProject<Projects.LedgerBot_Bot>("bot")
    .WithEnvironment("Telegram__BotToken", telegramBotToken);

var app = builder.Build();

await app.RunAsync();

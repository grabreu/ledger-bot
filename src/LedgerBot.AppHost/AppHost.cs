var builder = DistributedApplication.CreateBuilder(args);

var telegramBotToken = builder.AddParameter("telegram-bot-token", secret: true);

var llmEndpoint = builder.AddParameter("llm-endpoint");
var llmDeployment = builder.AddParameter("llm-deployment");
var llmApiKey = builder.AddParameter("llm-api-key", secret: true);

var sql = builder.AddSqlServer("sql")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("ledgerbot");

builder.AddDotnetProject("api", "../LedgerBot.Api/LedgerBot.Api.csproj")
    .WithReference(db)
    .WithEnvironment("Telegram__BotToken", telegramBotToken)
    .WithEnvironment("Llm__Endpoint", llmEndpoint)
    .WithEnvironment("Llm__Deployment", llmDeployment)
    .WithEnvironment("Llm__ApiKey", llmApiKey)
    .WaitFor(db);

var app = builder.Build();

await app.RunAsync();

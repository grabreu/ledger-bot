using LedgerBot.Bot;
using LedgerBot.Ledger;
using LedgerBot.ServiceDefaults;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Ledger Bot API",
            Version = "v1",
            Description = "Expense tracker driven by a Telegram bot and an LLM.",
            Contact = new()
            {
                Name = "Gabriel Abreu",
                Url = new("https://grabreu.dev")
            }
        };
        document.Servers = [];
        return Task.CompletedTask;
    });
});

builder.AddBotModuleServices();
builder.AddLedgerModuleServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();
app.Map("/", () => Results.Redirect("/scalar"));

await app.MigrateLedgerModuleDatabaseAsync();

await app.RunAsync();

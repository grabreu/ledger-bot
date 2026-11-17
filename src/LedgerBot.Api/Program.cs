using LedgerBot.Bot;
using LedgerBot.Ledger;
using LedgerBot.Llm;
using LedgerBot.ServiceDefaults;

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
            Description = "Household expense tracker driven by a Telegram bot and an LLM.",
            Contact = new OpenApiContact
            {
                Name = "Gabriel Abreu",
                Url = new Uri("https://grabreu.dev")
            }
        };
        document.Servers = [];
        return Task.CompletedTask;
    });
});

builder.Services.AddSingleton(TimeProvider.System);

builder.AddBotModuleServices();
builder.AddLlmModuleServices();
builder.AddLedgerModuleServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();
app.Map("/", () => Results.Redirect("/scalar"));

app.MapBotModuleEndpoints();

await app.EnsureLedgerModuleDatabaseAsync();

await app.RunAsync();

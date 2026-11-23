# ledger-bot

[![License](https://img.shields.io/github/license/grabreu/ledger-bot?style=flat-square)](LICENSE)

An expense tracker driven by a chat bot (Telegram first) and an LLM that acts only through validated tools. Write "gastei 45 no mercado" and it becomes a ledger entry; ask "quanto gastei com mercado em outubro?" and it answers from your own data.

_Backend only, on purpose: there is no dashboard. The bot is the interface._

> Status: early development. Entries are not saved from the bot yet.

## Tech stack

In use: .NET (ASP.NET Core), modular monolith with Vertical Slice Architecture · EF Core + SQL Server · Telegram Bot API (webhook in production, polling locally) · Azure OpenAI (`gpt-4.1-nano`) through `Microsoft.Extensions.AI` · .NET Aspire (local development) · OpenTelemetry

Planned: Azure Container Apps

## Implemented

- Telegram updates through a webhook (secret token validation) or polling for local development; messages are answered by the LLM service.
- `Llm` module: an `ILlmService` backed by an Azure OpenAI chat client with function calling, a scoped system prompt (budget topics only) and tools to get the current date, record an entry, and get a total or list entries.
- `Ledger` module: `Entry` schema with EF Core migrations (applied on startup) and use cases to create and delete entries and to get a total or list them. Delete is not exposed to the bot yet.

## Planned features

- **Natural-language entries**: income and expenses in PT-BR, saved through a validated command, with an Undo button. Entries are saved right away, with no confirmation step.
- **Questions about your data**: answered through read-only typed tools.
- **Shared ledger**: one ledger per Telegram chat; add the bot to a group to share it.
- **Idempotent Telegram updates**: processed at most once per `update_id`.

The reasoning behind these decisions will live in `docs/adr/`.

## Development

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

Run locally through the AppHost (needs Docker, a Telegram bot token and an Azure OpenAI deployment; the bot runs in polling mode):

```bash
dotnet run --project src/LedgerBot.AppHost
```

The AppHost asks for the `telegram-bot-token`, `llm-endpoint` and `llm-deployment` parameters and starts SQL Server and the API. `llm-endpoint` is the resource endpoint (`https://<resource>.openai.azure.com/`) and `llm-deployment` is the deployment name, not the model name. The API authenticates to Azure OpenAI with `DefaultAzureCredential` (no key): locally it uses your `az login` or IDE sign-in, which needs the Cognitive Services OpenAI User role on the resource.

## Deployment

TODO: Azure Container Apps via GitHub Actions with an OIDC federated credential, as in the other projects.

## License

Licensed under the [MIT License](LICENSE).

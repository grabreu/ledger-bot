# ledger-bot

[![License](https://img.shields.io/github/license/grabreu/ledger-bot?style=flat-square)](LICENSE)

A household expense tracker driven by a chat bot (Telegram first) and an LLM that never writes data. Write "gastei 45 no mercado" and it becomes a ledger entry; ask "quanto gastei com mercado em outubro?" and it answers from your own data.

_Backend only, on purpose: there is no dashboard. The bot is the interface and a GraphQL API sits underneath it._

> Status: early development. Entries are not saved from the bot yet.

## Tech stack

In use: .NET (ASP.NET Core), modular monolith with Vertical Slice Architecture · EF Core + SQL Server · Telegram Bot API (webhook) · .NET Aspire (local development) · OpenTelemetry

Planned: HotChocolate (GraphQL) · Gemini API behind a provider abstraction · Azure Container Apps

## Implemented

- Telegram webhook with secret token validation.
- Message interpretation behind an LLM provider abstraction, with a fake provider; the bot replies with the interpreted entry.
- `Ledger` module with the `LedgerEntry` schema and EF Core migrations (not yet wired to the bot).

## Planned features

- **Natural-language entries**: income and expenses in PT-BR, saved with an Undo button, or confirmed first when the amount is missing, the category is new, or the date is not today.
- **Questions about your data**: answered through read-only typed tools; the model never writes.
- **Households**: one shared ledger per household; the owner manages members and deletion.
- **Idempotent Telegram updates**: processed at most once per `update_id`.

The reasoning behind these decisions will live in `docs/adr/`.

## Development

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

Run locally through the AppHost (needs Docker, a Telegram bot token, and a Dev Tunnel for the webhook):

```bash
dotnet run --project src/LedgerBot.AppHost
```

The AppHost asks for the `telegram-bot-token` parameter and starts SQL Server, the API and the tunnel.

## Deployment

TODO: Azure Container Apps via GitHub Actions with an OIDC federated credential, as in the other projects.

## License

Licensed under the [MIT License](LICENSE).

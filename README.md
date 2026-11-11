# ledger-bot

[![License](https://img.shields.io/github/license/grabreu/ledger-bot?style=flat-square)](LICENSE)

A household expense tracker driven by a chat bot (Telegram first) and an LLM that never writes data. Write "gastei 45 no mercado" and it becomes a ledger entry; ask "quanto gastei com mercado em outubro?" and it answers from your own data.

_Backend only, on purpose: there is no dashboard. The bot is the interface and a GraphQL API sits underneath it._

> Status: in planning. Nothing below is implemented yet.

## Tech stack

Planned: .NET (ASP.NET Core), modular monolith with Vertical Slice Architecture · EF Core + SQL Server · HotChocolate (GraphQL) · Telegram Bot API · Gemini API behind a provider abstraction · OpenTelemetry · Azure Container Apps

## Planned features

- **Natural-language entries**: income and expenses in PT-BR, saved with an Undo button, or confirmed first when the amount is missing, the category is new, or the date is not today.
- **Questions about your data**: answered through read-only typed tools; the model never writes.
- **Households**: one shared ledger per household; the owner manages members and deletion.
- **Idempotent Telegram updates**: processed at most once per `update_id`.

The reasoning behind these decisions will live in `docs/adr/`.

## Development

TODO: filled in when the project is scaffolded.

## Deployment

TODO: Azure Container Apps via GitHub Actions with an OIDC federated credential, as in the other projects.

## License

Licensed under the [MIT License](LICENSE).

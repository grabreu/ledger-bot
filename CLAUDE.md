# ledger-bot

## Repository

An expense tracker driven by a Telegram bot and an LLM. Messages in natural language (PT-BR) become ledger entries; questions are answered from the user's own data. Backend only by design: no frontend, the bot is the interface. The LLM acts only through typed tools; writes go through a command that code validates (see the LLM rules below).

Read `README.md` before making changes: it documents the project pitch and how to run it. Significant, hard-to-reverse decisions are recorded in `docs/adr/`: check it before revisiting one, and add an entry when making a new one. The design draft (domain model and technical decisions) lives in the private `rascunho` repository under `ideas/ledger-bot/`.

## General Rules

- Keep changes scoped to the requested change.
- Prefer existing patterns over introducing new abstractions.
- Do not add dependencies unless they are necessary.
- Do not fill gaps with assumptions when the user hasn't given the information: ask, or mark it as pending.
- Do not claim a validation command passed unless it was actually run.
- Code, comments, commit messages, and documentation are always written in English.
- Do not use em dashes; use a comma, colon, semicolon, parentheses, or a separate sentence instead.

## Git

- Do not create or switch branches unless explicitly requested.
- Do not create commits unless explicitly requested.
- Do not push unless explicitly requested.
- Keep commits focused on the requested change.
- Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/) (`type: summary`).

## Documentation

### Audience

Future-you revisiting this months later, or someone browsing the portfolio to see how it works. Not onboarding material; keep it concise and skimmable.

### Content Rules

- State facts concisely. Avoid unnecessary explanations or trailing rationale.
- Do not document information that is already obvious from the repository structure or configuration.
- Do not invent features, API shapes, or future direction: mark undecided things as TODO.
- Document a capability only after it is implemented and verified.
- Use proper Markdown headings (`##`, `###`), not bold text as headings.

---

## Project-Specific Guidelines

### Source

.NET (ASP.NET Core), modular monolith with Vertical Slice Architecture: one slice per use case, with command or query, handler, and endpoint colocated. Modules: Ledger, Reminders, Llm; `Api` is the host (Telegram webhook). Modules talk through the commands, queries and DTOs of their `*.Contracts` project (dispatched with Mediator, returning `Result` from Desfecho), never by referencing each other's internals. One SQL Server database, one schema per module, no foreign keys across schemas.

Layout (one project per module, plus a `*.Contracts` project each; only the hosts, `Bot`, `Ledger` and `Llm` exist so far, modules are added with their first slice):

- `src/LedgerBot.AppHost`: .NET Aspire host for local development only (no deploy through Aspire).
- `src/LedgerBot.ServiceDefaults`: OpenTelemetry, health checks, resilience.
- `src/LedgerBot.Api`: host and composition root only (Program, API docs).
- `src/LedgerBot.Bot`: one folder per use case in `Features/` (handler, plus command or query when needed) and the Telegram plumbing in `Telegram/` (options, update dispatcher, webhook endpoint with secret validation and webhook registration, polling service). Handlers use `ITelegramBotClient` directly.
- `src/LedgerBot.Ledger` (Entry)
- `src/LedgerBot.Llm` (`ILlmService` over an `IChatClient`, tool sets, LlmCall)
- `src/LedgerBot.Reminders` (Reminder; v1.1)
- `tests/LedgerBot.*.Tests`

Rules that hold regardless of layout:

- LLM tools are the router: read tools run directly; a write tool (add entry) runs a Ledger command, and the chat, user and validation (amount, date) are fixed in code, never taken from model output. Undo comes from a button. The Bot builds buttons from structured events, not from model text.
- Commands and queries are validated by a FluentValidation validator in a Mediator pipeline behavior (`Ledger/ValidationBehavior`, registered by the Ledger module).
- Basic model first: `CreateEntryCommand` returns `Result<Guid>` (the entry id, or validation errors), with no confirmation step in v1 (decided). After v1, confirmation would be a deterministic rule, not a model-reported confidence. Idea explored in a spike: the write tool only proposes, the user confirms with buttons, and proposals live in a distributed cache with a TTL (for example `IDistributedCache` on SQL Server, no extra cost) instead of a pending status in the Entry table; confirming would reuse the proposal id as the entry id so a double click fails on the primary key.
- TODO: delete and edit need user approval (button or approval-required tool), never model output alone; one write per message (guard against duplicate tool calls); description length limit.
- Telegram has two modes (`Telegram:Mode`): `Webhook` (production, the default in `appsettings.json`) and `Polling` (local development, set in `appsettings.Development.json`, no public URL or tunnel needed). Both share one update dispatcher. Updates are processed on the spot (in the request, or in the polling callback); pending confirmations live in the database. In webhook mode validate `X-Telegram-Bot-Api-Secret-Token` on every request; the endpoint is not mapped in polling mode.
- Use `Ledger` and `Entry` (with `EntryKind`: `Income` | `Expense`) for the domain, never "Transaction" (it collides with database transactions).
- Entry dates are `DateOnly` in `America/Sao_Paulo`; audit timestamps are UTC.
- Telegram is the only channel, with no channel abstraction; a user is identified by the Telegram user id. Entries are scoped by Telegram chat id (private chat: personal ledger; group: shared ledger), see `docs/adr/0001-scope-the-ledger-by-telegram-chat.md`.
- Never log message content; LLM calls record model, prompt version, tokens, estimated cost, latency, and outcome only.
- Secrets (bot token, webhook secret) come from configuration, never from code. The LLM has no key: the API authenticates to Azure OpenAI with `DefaultAzureCredential` (Microsoft Entra ID).

### Validation

Matches CI (`.github/workflows/ci.yml`):

```bash
dotnet restore
dotnet format --no-restore --verify-no-changes --severity info
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
```

### Tests

xunit v3, run through Microsoft.Testing.Platform (`global.json`). Three layers; do not repeat the same rule in more than one:

- Unit: only where needed (domain rules). Named `Method_ShouldX_WhenY`.
- Functional: dispatch a command or query of a module and assert the result. One test project per module, with `Unit/` and `Functional/` folders. They run against a real SQL Server in a Testcontainers container.
- Acceptance: conversation flows through the bot, only in `LedgerBot.Bot.Tests`, with the Telegram Bot API and the LLM provider faked. TODO: Reqnroll (Gherkin) to be confirmed once the first flows exist.

Infrastructure smoke tests (for example `/health`) stay as plain xunit tests.

### Open Questions

- TODO: dedup of Telegram updates by `update_id` (SQL table with id and status, no message content) before production. Until then updates are processed on the spot. The original message text is not stored with entries.
- LLM provider: Azure OpenAI (`gpt-4.1-nano`, Global Standard deployment), pay as you go on the existing Azure subscription; the deployment works and a request to raise the quota to 10,000 TPM is under review. Gemini was dropped (it asked for a BRL 170 prepaid deposit). TODO: confirm the Azure OpenAI data-use terms (training, retention) in the official docs.
- Deploy starts with `minReplicas=0` (no compute cost within the free grants; `minReplicas=1` would be about USD 10 (roughly BRL 54) per month against a BRL 50 budget). Only safe after the `update_id` dedup, because a cold start may make Telegram resend an update. Keep at 0 unless the cold start gets in the way and the budget changes. TODO: check the "Environment Management Hour" meter in the first month's cost analysis.

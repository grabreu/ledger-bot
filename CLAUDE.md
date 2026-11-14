# ledger-bot

## Repository

A household expense tracker driven by a Telegram bot and an LLM. Messages in natural language (PT-BR) become ledger entries; questions are answered from the household's own data. Backend only by design: no frontend, the bot is the interface and a GraphQL API (HotChocolate) sits underneath it. The LLM never writes data: it calls read-only typed tools or returns an interpreted entry that code validates and saves.

Read `README.md` before making changes: it documents the project pitch and how to run it. Significant, hard-to-reverse decisions are recorded in `docs/adr/`: check it before revisiting one, and add an entry when making a new one. The design draft (domain model, technical decisions, Telegram research) lives in the private `rascunho` repository under `ideas/ledger-bot/`.

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

.NET (ASP.NET Core), modular monolith with Vertical Slice Architecture: one slice per use case, with command or query, handler, and endpoint colocated. Modules: Households, Ledger, Reminders, Llm; the bot is the host (webhook, inbox, background worker). Modules talk through events and `*.Contracts` interfaces, never by referencing each other's internals. One SQL Server database, one schema per module, no foreign keys across schemas.

Layout (one project per module, plus a `*.Contracts` project each; only the host projects exist so far, modules are added with their first slice):

- `src/LedgerBot.AppHost`: .NET Aspire host for local development only (no deploy through Aspire).
- `src/LedgerBot.ServiceDefaults`: OpenTelemetry, health checks, resilience.
- `src/LedgerBot.Bot`: host (webhook, inbox, worker, pending confirmations, GraphQL). Channels live inside it: `Channels/` holds the channel interface and shared types, `Channels/Telegram/` the first adapter. Bot flow code uses only the interface, never Telegram types. A new channel is a new folder, moved to its own project only if it grows.
- `src/LedgerBot.Households` (User, Household, Membership, Invite)
- `src/LedgerBot.Ledger` (Category, LedgerEntry)
- `src/LedgerBot.Llm` (provider abstraction, read-only tools, LlmCall)
- `src/LedgerBot.Reminders` (Reminder; v1.1)
- `tests/LedgerBot.*.Tests`

Rules that hold regardless of layout:

- The LLM never writes. It calls read-only typed tools, or returns an interpreted entry that code validates (amount, category, date) and saves. Undo and delete come from buttons, never from model output.
- Entries are saved directly with an Undo button; confirmation is required first when the amount is missing, the category does not exist yet, or the date is not today (a deterministic rule, not a model-reported confidence).
- Telegram updates are written to an inbox table by `update_id` before answering 200 and processed by a background worker; pending confirmations live in the database. Validate `X-Telegram-Bot-Api-Secret-Token` on every request.
- Use `Ledger` and `LedgerEntry` for the domain, never "Transaction" (it collides with database transactions).
- Entry dates are `DateOnly` in `America/Sao_Paulo`; audit timestamps are UTC.
- The bot talks to the domain through a channel interface so another channel (WhatsApp) can be added; a user is identified by channel plus the channel's own user id.
- Never log message content; LLM calls record model, prompt version, tokens, estimated cost, latency, and outcome only.
- Secrets (bot token, webhook secret, LLM key) come from configuration, never from code.

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
- Functional: dispatch a command or query of a module and assert the result. One test project per module, with `Unit/` and `Functional/` folders. TODO: database for these tests (SQL Server container vs. alternatives).
- Acceptance: conversation flows through the bot, only in `LedgerBot.Bot.Tests`, with a fake channel and a fake LLM provider. TODO: Reqnroll (Gherkin) to be confirmed once the first flows exist.

Infrastructure smoke tests (for example `/health`) stay as plain xunit tests.

### Open Questions

- TODO: retention of the original message stored with each entry (personal data).
- TODO: confirm in the official Gemini API terms that enabling billing excludes the data from product improvement.
- TODO: cost of `minReplicas=1` against the `azure-infra` monthly budget.
- TODO: how an invite code travels on WhatsApp, if that channel is added.

# Scope the ledger by Telegram chat

Entries belong to a Telegram chat (`ChatId`), not to a household: a private chat is a personal ledger, and adding the bot to a group makes it shared. The planned `Households` module (users, memberships, invites) is dropped because it adds identity and invite rules that v1.0 does not need. `CreatedBy` still records the Telegram user who wrote each entry.

**Consequences**: who can read a shared ledger is whoever Telegram lets into the group, and the history has no owner if the chat goes away. A household concept can be added later by mapping `ChatId` to it. TODO: confirm the group privacy mode (BotFather) so the bot sees plain messages in groups.

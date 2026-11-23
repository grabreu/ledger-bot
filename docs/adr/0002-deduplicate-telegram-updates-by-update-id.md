# Deduplicate Telegram updates by update_id

The dispatcher claims each update by inserting its `update_id` into `Bot.ProcessedUpdates` (primary key) before processing it. A duplicate key means Telegram redelivered the update, and it is ignored. The row holds the id, a status (`Processing`, `Completed`, `Failed`) and the time it was received, never message content. This is needed before running with `minReplicas=0`, because a cold start can make Telegram resend an update.

**Consequences**: processing is at most once. If the app dies while an update is `Processing`, or the processing throws, the redelivery is ignored and the user has to send the message again; reprocessing instead could save an entry twice. The status is for diagnosis only. Old rows are not purged yet. TODO: purge old rows.

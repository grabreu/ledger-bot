namespace LedgerBot.Bot.Domain;

public sealed class ProcessedUpdate
{
    private ProcessedUpdate()
    {
    }

    public ProcessedUpdate(long updateId, DateTimeOffset receivedAt)
    {
        UpdateId = updateId;
        Status = ProcessedUpdateStatus.Processing;
        ReceivedAt = receivedAt;
    }

    public long UpdateId { get; private set; }
    public ProcessedUpdateStatus Status { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
}

namespace LedgerBot.Bot.Telegram;

public enum TelegramMode { Polling, Webhook }

public sealed class TelegramOptions : IValidatableObject
{
    public const string SectionName = "Telegram";

    [Required] public string BotToken { get; set; } = string.Empty;
    public TelegramMode Mode { get; init; } = TelegramMode.Polling;
    public string WebhookUrl { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Mode == TelegramMode.Webhook)
        {
            if (string.IsNullOrWhiteSpace(WebhookUrl))
            {
                yield return new ValidationResult("The WebhookUrl field is required when Mode is set to Webhook.", [nameof(WebhookUrl)]);
            }
            if (string.IsNullOrWhiteSpace(WebhookSecret))
            {
                yield return new ValidationResult("The WebhookSecret field is required when Mode is set to Webhook.", [nameof(WebhookSecret)]);
            }
        }
    }
}

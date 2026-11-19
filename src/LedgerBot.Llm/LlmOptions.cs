using System.ComponentModel.DataAnnotations;

namespace LedgerBot.Llm;

public sealed class LlmOptions
{
    public const string SectionName = "Llm";

    [Required, Url] public string Endpoint { get; set; } = string.Empty;
    [Required] public string Deployment { get; set; } = string.Empty;
    [Required] public string ApiKey { get; set; } = string.Empty;
}

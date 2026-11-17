using LedgerBot.Bot.Features.InterpretMessage;

namespace LedgerBot.Bot.Telegram;

public static class TelegramWebhookEndpoint
{
    public const string Route = "/telegram/webhook";

    private const string SecretHeader = "X-Telegram-Bot-Api-Secret-Token";

    public static RouteHandlerBuilder MapTelegramWebhookEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost(Route, HandleAsync)
            .WithTags("Telegram")
            .WithName("TelegramWebhook")
            .WithSummary("Telegram Webhook")
            .WithDescription("Receives updates from Telegram. Requires the secret token header.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    // Provisional: the update is handled inside the request until the inbox exists.
    private static async Task<IResult> HandleAsync(
        HttpContext context,
        IOptions<TelegramSettings> options,
        InterpretMessageHandler handler,
        CancellationToken cancellationToken)
    {
        if (!HasValidSecret(context.Request, options.Value.WebhookSecret))
        {
            return Results.Unauthorized();
        }

        var update = await context.Request.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);

        if (update?.Message is { From: not null, Text: { } text } message && !string.IsNullOrWhiteSpace(text))
        {
            await handler.HandleAsync(message.Chat.Id, text, cancellationToken);
        }

        return Results.Ok();
    }

    private static bool HasValidSecret(HttpRequest request, string expected)
    {
        var provided = request.Headers[SecretHeader].ToString();

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected));
    }
}

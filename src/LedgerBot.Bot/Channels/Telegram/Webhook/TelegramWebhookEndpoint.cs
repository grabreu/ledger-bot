using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Channels.Telegram.Webhook;

public static class TelegramWebhookEndpoint
{
    public const string Path = "/telegram/webhook";

    private const string SecretHeader = "X-Telegram-Bot-Api-Secret-Token";

    public static RouteHandlerBuilder MapTelegramWebhookEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost(Path, HandleAsync)
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
        IOptions<TelegramOptions> options,
        IIncomingMessageHandler handler,
        CancellationToken cancellationToken)
    {
        if (!HasValidSecret(context.Request, options.Value.WebhookSecret))
        {
            return Results.Unauthorized();
        }

        var update = await context.Request.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);
        var incoming = update?.Message?.ToIncomingMessage();
        if (incoming is null)
        {
            return Results.Ok();
        }

        await handler.HandleAsync(incoming, cancellationToken);

        return Results.Ok();
    }

    private static bool HasValidSecret(HttpRequest request, string expected)
    {
        var provided = request.Headers[SecretHeader].ToString();

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected));
    }
}

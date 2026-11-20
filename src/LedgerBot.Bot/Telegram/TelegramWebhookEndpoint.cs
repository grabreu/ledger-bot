using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

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

    private static async Task<IResult> HandleAsync(
        HttpRequest request,
        Update update,
        TelegramUpdateDispatcher dispatcher,
        IOptions<TelegramOptions> options,
        CancellationToken cancellationToken)
    {
        if (!HasValidSecret(request, options.Value.WebhookSecret))
        {
            return Results.Unauthorized();
        }

        await dispatcher.DispatchAsync(update, cancellationToken);

        return Results.Ok();
    }

    private static bool HasValidSecret(HttpRequest request, string expected)
    {
        var provided = request.Headers[SecretHeader].ToString();
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected));
    }
}

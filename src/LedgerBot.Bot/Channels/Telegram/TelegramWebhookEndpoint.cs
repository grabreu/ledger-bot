using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace LedgerBot.Bot.Channels.Telegram;

public static class TelegramWebhookEndpoint
{
    public const string Path = "/telegram/webhook";

    private const string SecretHeader = "X-Telegram-Bot-Api-Secret-Token";

    public static WebApplication MapTelegramWebhook(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<TelegramOptions>>().Value;
        if (options.Mode != TelegramMode.Webhook)
        {
            return app;
        }

        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("LedgerBot.Bot.Channels.Telegram.Webhook");

        // Provisional: the update is handled inside the request until the inbox exists.
        app.MapPost(Path, async (HttpContext context, CancellationToken cancellationToken) =>
        {
            if (!HasValidSecret(context.Request, options.WebhookSecret))
            {
                return Results.Unauthorized();
            }

            var update = await context.Request.ReadFromJsonAsync<Update>(JsonBotAPI.Options, cancellationToken);
            var incoming = update?.Message is { } message ? TelegramMessageMapper.ToIncomingMessage(message) : null;
            if (incoming is null)
            {
                return Results.Ok();
            }

            try
            {
                var handler = context.RequestServices.GetRequiredService<IIncomingMessageHandler>();
                await handler.HandleAsync(incoming, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to handle a Telegram message");
            }

            return Results.Ok();
        }).ExcludeFromDescription();

        return app;
    }

    private static bool HasValidSecret(HttpRequest request, string expected)
    {
        var provided = request.Headers[SecretHeader].ToString();

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected));
    }
}

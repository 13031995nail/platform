namespace Tmobis.Integrations.Messengers.Telegram.Authorization;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public sealed class TelegramWebhookAuthorizationHandler(
    IOptions<TelegramOptions> optionsAccessor)
    : AuthorizationHandler<TelegramWebhookAuthorizationRequirement>
{
    public const string SecretHeader = "X-Telegram-Bot-Api-Secret-Token";

    private readonly TelegramOptions options = optionsAccessor.Value;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TelegramWebhookAuthorizationRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        var actualSecret = httpContext.Request.Headers[SecretHeader].ToString();
        if (SecretsMatch(options.WebhookSecret, actualSecret))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    internal static bool SecretsMatch(string expected, string actual)
    {
        if (string.IsNullOrWhiteSpace(expected)
            || string.IsNullOrWhiteSpace(actual))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var actualBytes = Encoding.UTF8.GetBytes(actual);
        return expectedBytes.Length == actualBytes.Length
            && CryptographicOperations.FixedTimeEquals(
                expectedBytes,
                actualBytes);
    }
}

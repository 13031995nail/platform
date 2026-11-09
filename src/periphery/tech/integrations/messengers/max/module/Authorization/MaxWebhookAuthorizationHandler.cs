namespace Tmobis.Integrations.Messengers.Max.Authorization;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

public sealed class MaxWebhookAuthorizationHandler(
    IOptions<MaxOptions> options)
    : AuthorizationHandler<MaxWebhookAuthorizationRequirement>
{
    public const string SecretHeader = "X-Max-Bot-Api-Secret";

    private readonly MaxOptions options = options.Value;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MaxWebhookAuthorizationRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        var expectedSecret = options.WebhookSecret;
        var actualSecret = httpContext.Request.Headers[SecretHeader].ToString();
        if (SecretsMatch(expectedSecret, actualSecret))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    internal static bool SecretsMatch(string expected, string actual)
    {
        if (string.IsNullOrEmpty(expected)
            || string.IsNullOrEmpty(actual))
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

namespace Tmobis.Integrations.Messengers.Vk;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Repositories;
using Tmobis.Integrations.Messengers.Vk.Messages;
using Tmobis.Medium.WebApi;

internal sealed class VkEndpointBuilder(
    IOptions<VkOptions> optionsAccessor)
    : IApiEndpointBuilder
{
    private readonly VkOptions options = optionsAccessor.Value;

    public void Build(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
                "/api/Vk/webhook",
                HandleCallback)
            .AllowAnonymous()
            .WithTags("Vk")
            .Accepts<VkCallbackReceived>("application/json")
            .Produces(StatusCodes.Status200OK, contentType: "text/plain")
            .Produces(StatusCodes.Status403Forbidden);
    }

    private async Task<IResult> HandleCallback(
        HttpRequest request,
        IMessageDispatcher messageDispatcher,
        IUnitOfWork unitOfWork)
    {
        using var reader = new StreamReader(request.Body);
        var json = await reader.ReadToEndAsync();
        VkCallbackReceived? callback;
        try
        {
            callback = JsonConvert.DeserializeObject<VkCallbackReceived>(json);
        }
        catch (JsonException)
        {
            return Results.BadRequest();
        }

        if (callback == null || !IsAuthorized(callback))
        {
            return Results.StatusCode(StatusCodes.Status403Forbidden);
        }

        if (string.Equals(
            callback.Type,
            "confirmation",
            StringComparison.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(options.ConfirmationCode))
            {
                return Results.Problem(
                    "Не задан Vk:ConfirmationCode",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            return Results.Text(options.ConfirmationCode, "text/plain");
        }

        await messageDispatcher.Send(callback with { Secret = string.Empty });
        await unitOfWork.Commit();
        return Results.Text("ok", "text/plain");
    }

    private bool IsAuthorized(VkCallbackReceived callback)
        => options.GroupId > 0
            && callback.GroupId == options.GroupId
            && SecretsMatch(options.CallbackSecret, callback.Secret);

    private static bool SecretsMatch(string expected, string actual)
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

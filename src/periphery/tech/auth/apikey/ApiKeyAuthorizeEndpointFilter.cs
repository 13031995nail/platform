namespace Tmobis.Auth.ApiKey;

using Microsoft.AspNetCore.Http;

/// <summary>
/// Endpoint filter для Minimal API, который применяет общую проверку ApiKey.
/// </summary>
public sealed class ApiKeyAuthorizeEndpointFilter(
  ApiKeyAuthorizationChecker checker)
: IEndpointFilter
{
  /// <summary>
  /// Останавливает обработку запроса с Unauthorized, если ApiKey не прошёл проверку.
  /// </summary>
  public async ValueTask<object?> InvokeAsync(
    EndpointFilterInvocationContext context,
    EndpointFilterDelegate next)
  {
    var metadata = context.HttpContext.GetEndpoint()?.Metadata
      ?? Enumerable.Empty<object>();

    if (!checker.IsAuthorized(context.HttpContext, metadata))
    {
      return Results.Unauthorized();
    }

    return await next(context);
  }
}

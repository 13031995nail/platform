namespace Tmobis.Auth.ApiKey;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

/// <summary>
/// Общая проверка ApiKey для MVC-фильтра и Minimal API endpoint filter-а.
/// </summary>
public sealed class ApiKeyAuthorizationChecker(
  IOptions<ApiKeyAuthorizationOptions> options)
{
  private readonly ApiKeyAuthorizationOptions options = options.Value;

  /// <summary>
  /// Проверяет AllowAnonymousAttribute, наличие заголовка ApiKey и совпадение ключа с настройками.
  /// </summary>
  public bool IsAuthorized(
    HttpContext httpContext,
    IEnumerable<object> endpointMetadata)
  {
    if (endpointMetadata.OfType<AllowAnonymousAttribute>().Any())
    {
      return true;
    }

    var apiKey = options.ApiKey;
    var apiKeyHeaderName = options.ApiKeyHeaderName;

    return !string.IsNullOrEmpty(apiKey)
      && !string.IsNullOrEmpty(apiKeyHeaderName)
      && httpContext.Request.Headers.TryGetValue(
        apiKeyHeaderName, out var extractedApiKey)
      && apiKey.Equals(
        extractedApiKey,
        StringComparison.InvariantCulture);
  }
}

namespace Tmobis.Auth.ApiKey;

public sealed class ApiKeyAuthorizationOptions
{
  public const string ApiKeyAuthorize = "ApiKeyAuthorize";

  public string ApiKey { get; set; }

  public string ApiKeyHeaderName { get; set; } = "ApiKey";
}

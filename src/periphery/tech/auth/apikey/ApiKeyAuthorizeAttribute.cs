namespace Tmobis.Auth.ApiKey;

using Microsoft.AspNetCore.Mvc;

public class ApiKeyAuthorizeAttribute()
: TypeFilterAttribute(typeof(ApiKeyAuthorizeFilter))
{
  /// <inheritdoc />
  public override string ToString() => "ApiKeyAuthorize";
}
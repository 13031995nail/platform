namespace Tmobis.Auth.ApiKey;

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

/// <summary>
/// MVC authorization filter для ApiKey, который делегирует проверку общему checker-у.
/// </summary>
public class ApiKeyAuthorizeFilter(
  ApiKeyAuthorizationChecker checker)
: IAuthorizationFilter
{
  /// <summary>
  /// Возвращает UnauthorizedResult, если запрос не прошёл проверку ApiKey.
  /// </summary>
  public void OnAuthorization(AuthorizationFilterContext context)
  {
    if (!checker.IsAuthorized(
      context.HttpContext,
      context.ActionDescriptor.EndpointMetadata))
    {
      context.Result = new UnauthorizedResult();
    }
  }
}

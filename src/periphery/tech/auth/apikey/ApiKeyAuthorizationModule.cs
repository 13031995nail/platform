namespace Tmobis.Auth.ApiKey;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;

/// <summary>
/// Модуль авторизации HTTP-запросов по API key.
/// Определяет проверку ключа из заголовка и policy авторизации для Web API.
/// </summary>
[ModuleRequirement(typeof(WebFrameworkModule))]
public class ApiKeyAuthorizationModule : ModuleBase
{
  /// <summary>
  /// Регистрирует настройки ApiKey, общий checker и endpoint filter для Minimal API.
  /// </summary>
  public override void Register(
    IServiceCollection services,
    IConfiguration configuration,
    IModuleRegister modules)
  {
    services
      .Configure<ApiKeyAuthorizationOptions>(
        configuration.GetSection(
            ApiKeyAuthorizationOptions.ApiKeyAuthorize));
    services.AddTransient<ApiKeyAuthorizationChecker>();
    services.AddTransient<ApiKeyAuthorizeEndpointFilter>();

    var policy = new AuthorizationPolicyBuilder()
        .RequireAssertion(ValidateApiKey).Build();
    services.AddAuthorization(options =>
    {
      options.DefaultPolicy = policy;
    });
  }

  private bool ValidateApiKey(AuthorizationHandlerContext context)
  {
    if (context.Resource is HttpContext httpContext)
    {
      var opt = httpContext.RequestServices
        .GetRequiredService<IOptions<ApiKeyAuthorizationOptions>>().Value;
      if (httpContext.Request.Headers.TryGetValue(opt.ApiKeyHeaderName, out var extracted))
      {
        return opt.ApiKey.Equals(extracted, StringComparison.InvariantCulture);
      }
    }
    return false;
  }
}

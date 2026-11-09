namespace Tmobis.Auth.UserSessions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Modules;
using Tmobis.Core.Modules;
using Tmobis.Frameworks.Web.Modules;
using System.Threading.Tasks;

/// <summary>
/// Модуль авторизации по пользовательской сессии.
/// Дополняет default authorization policy проверкой существования текущей сессии.
/// </summary>
[ModuleRequirement(typeof(WebFrameworkModule), typeof(UserSessionsModule))]
public class UserSessionAuthorizationModule : ModuleBase
{
    /// <summary>
    /// Регистрирует endpoint filter для Minimal API и настраивает политику авторизации.
    /// </summary>
    public override void Register(
        IServiceCollection services,
        IConfiguration configuration,
        IModuleRegister modules)
    {
        services.AddTransient<UserSessionAuthFilter>();

        services.AddAuthorization(options =>
        {
            var existingDefaultPolicy = options.DefaultPolicy;
            var combinedPolicyBuilder = new AuthorizationPolicyBuilder(existingDefaultPolicy);
            combinedPolicyBuilder.RequireAssertion(ValidateSession);
            options.DefaultPolicy = combinedPolicyBuilder.Build();
        });
    }

    /// <summary>
    /// Проверяет наличие текущей сессии пользователя.
    /// </summary>
    /// <param name="context">Контекст авторизации.</param>
    /// <returns>true, если сессия найдена; иначе false.</returns>
    private async Task<bool> ValidateSession(AuthorizationHandlerContext context)
    {
        var httpContext = context.Resource switch
        {
            HttpContext hc => hc,
            Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext filterCtx => filterCtx.HttpContext,
            Endpoint => null,
            _ => null
        };

        if (httpContext is null)
        {
            return false;
        }

        var sessionProvider = httpContext.RequestServices.GetRequiredService<IUserSessionProvider>();

        var session = await sessionProvider.GetCurrentSession();
        return session != null;
    }
}

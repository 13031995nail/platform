namespace Tmobis.Auth.UserSessions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Tmobis.Business.Common.Sessions.Providers;

/// <summary>
/// Endpoint filter для Minimal API, который применяет общую проверку пользовательской сессии.
/// </summary>
public sealed class UserSessionAuthFilter(
    IUserSessionProvider sessionProvider)
: IEndpointFilter
{
    /// <summary>
    /// Останавливает обработку запроса с Unauthorized, если сессия не прошла проверку.
    /// </summary>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var metadata = context.HttpContext.GetEndpoint()?.Metadata
            ?? Enumerable.Empty<object>();

        if (metadata.OfType<AllowAnonymousAttribute>().Any())
        {
            return await next(context);
        }

        var session = await sessionProvider.GetCurrentSession();
        if (session == null)
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }
}

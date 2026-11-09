namespace Tmobis.Frameworks.Web.Backend.Users;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

public class CurrentUser(
    IHttpContextAccessor httpContextAccessor)
: ICurrentUser
{
    /// <inheritdoc/>
    public virtual string Login => GetLoginFromContext() ?? "system";

    /// <summary>
    /// Получить логин текущего пользователя из HttpContext.
    /// </summary>
    /// <returns>Логин текущего пользователя.</returns>
    protected virtual string? GetLoginFromContext()
    {
        var principal = httpContextAccessor?.HttpContext?.User;
        var login = principal?.Identity?.Name;
        if (string.IsNullOrEmpty(login))
        {
            login = principal?.Claims
                .FirstOrDefault(claim => claim.Type == "unique_name")?
                .Value;
        }

        if (!string.IsNullOrEmpty(login)) return login;
        
        try
        {
            StringValues pair = default;
            var hasInForm = httpContextAccessor?.HttpContext?
                .Request?.Form?.TryGetValue("username", out pair) ?? false;
            if (hasInForm)
            {
                login = pair.FirstOrDefault();
            }
        }
        catch (InvalidOperationException)
        {
            // ignored
        }

        return login;
    }
}

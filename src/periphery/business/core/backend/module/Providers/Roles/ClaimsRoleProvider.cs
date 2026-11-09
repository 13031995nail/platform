namespace Tmobis.Business.Core.Providers.Roles;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

/// <summary>
/// Сервис получения ролей из claims текущего пользователя.
/// </summary>
public class ClaimsRoleProvider(IHttpContextAccessor? httpContextAccessor = null) : ICurrentRoleProvider
{
  private readonly IHttpContextAccessor? httpContextAccessor = httpContextAccessor;

  /// <inheritdoc/>
  public ValueTask<List<string>> GetCurrentRoleNames()
  {
    var principal = httpContextAccessor?.HttpContext?.User;
    if (principal == null)
    {
      return ValueTask.FromResult<List<string>>([]);
    }

    return ValueTask.FromResult<List<string>>(
      [.. principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value)]);
  }
}

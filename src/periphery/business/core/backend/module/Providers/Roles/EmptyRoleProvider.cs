namespace Tmobis.Business.Core.Providers.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис получения ролей.
/// </summary>
// TODO: Реализовать на основе IRepository
public class EmptyRoleProvider : IRoleProvider
{
  /// <inheritdoc/>
  public virtual ValueTask<List<string>> GetRoleNamesByUser(IUser? user)
  => ValueTask.FromResult<List<string>>([]);

  public virtual async ValueTask<bool> HasRole(IUser? user, string roleName)
  {
    var roles = await GetRoleNamesByUser(user);
    return roles.Contains(roleName);
  }

  public async ValueTask<bool> IsAdmin(IUser user)
  {
    var roles = await GetRoleNamesByUser(user);
    return roles.Contains("admin") || roles.Contains("Администратор");
  }

  public async ValueTask<bool> IsSuperAdmin(IUser user)
  {
    var roles = await GetRoleNamesByUser(user);
    return roles.Contains("admin");
  }

  public virtual Task GrantRole(IUser user, string roleName)
  => Task.CompletedTask;
}

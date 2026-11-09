namespace Tmobis.Business.Core.Providers.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис получения ролей.
/// </summary>
public interface IRoleProvider
{
  /// <summary>
  /// Получить все роли пользователя,
  /// включая наследуемые от непосредственных ролей пользователя роли.
  /// </summary>
  /// <returns>Объект пользователя.</returns>
  public ValueTask<List<string>> GetRoleNamesByUser(IUser? user);

  public ValueTask<bool> HasRole(IUser? user, string roleName);

  public ValueTask<bool> IsAdmin(IUser user);

  public ValueTask<bool> IsSuperAdmin(IUser user);

  public Task GrantRole(IUser user, string roleName);
}

namespace Tmobis.Business.Core.Providers.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users;

/// <summary>
/// Сервис получения ролей текущего пользователя через провайдер ролей.
/// </summary>
public class RoleProviderCurrentRoleProvider(
  ICurrentUserProvider currentUserProvider,
  IRoleProvider roleProvider)
: ICurrentRoleProvider
{
  /// <inheritdoc/>
  public async ValueTask<List<string>> GetCurrentRoleNames()
  {
    var currentUser = await currentUserProvider.GetCurrentUser();
    return await roleProvider.GetRoleNamesByUser(currentUser);
  }
}

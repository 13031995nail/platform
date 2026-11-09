namespace Tmobis.Business.Core.Providers.Roles;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Сервис получения ролей текущего пользователя.
/// </summary>
public interface ICurrentRoleProvider
{
  /// <summary>
  /// Получить все роли текущего пользователя.
  /// </summary>
  /// <returns>Имена ролей текущего пользователя.</returns>
  public ValueTask<List<string>> GetCurrentRoleNames();
}

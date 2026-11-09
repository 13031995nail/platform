namespace Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис получения пользователей.
/// </summary>
public interface ICurrentUserProvider
{
  /// <summary>
  /// Получить текущего пользователя.
  /// </summary>
  /// <returns>Объект текущего пользователя.</returns>
  public Task<IUser?> GetCurrentUser();

  /// <summary>
  /// Получить сотрудника текущего пользователя.
  /// </summary>
  /// <returns>Объект текущего сотрудника.</returns>
  public Task<IEmployee?> GetCurrentEmployee();

  /// <summary>
  /// Получить данные о подключении (IP, etc)
  /// текущего пользователя.
  /// </summary>
  /// <returns>Данные о подключении.</returns>
  public string? GetUserConnectionInfo();

  /// <summary>
  /// Получить адрес текущего пользователя.
  /// </summary>
  /// <returns>Адрес пользователя.</returns>
  public string? GetUserConnectionAddress();
}
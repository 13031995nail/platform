namespace Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.Models.Towns;
using Tmobis.Business.Core.Models.Users;
using System.Collections.Generic;
using Tmobis.Business.Core.Models.Employees;

/// <summary>
/// Сервис получения пользователей.
/// </summary>
public interface IUserProvider
{
  /// <summary>
  /// Получить пользователя по логину.
  /// </summary>
  /// <param name="login">Логин пользователя.</param>
  /// <returns>Объект пользователя.</returns>
  public Task<IUser?> GetUserByLogin(string login);

  /// <summary>
  /// Получить пользователя по ID.
  /// </summary>
  /// <param name="id">ID.</param>
  /// <returns>Объект пользователя.</returns>
  public ValueTask<IUser?> GetUserById(Guid id);

  /// <summary>
  /// Получить список пользователей мультипользователя
  /// по логину и ID мультипользователя.
  /// </summary>
  /// <param name="userId">ID мультипользователя.</param>
  /// <param name="login">Логин пользователя.</param>
  /// <returns>Список пользователей мультипользователя.</returns>
  public Task<List<IUser>?> GetMultiUsers(string userId, string login);

  /// <summary>
  /// Получить admin по текущему филиалу.
  /// </summary>
  /// <returns>Объект пользователя admin.</returns>
  public Task<IUser?> GetAdminByTown(ITown town);

  /// <summary>
  /// Проверка, обычно нужная для понимания, может ли пользователь
  /// выполнять операцию над другим пользователем:
  /// если он является администратором системы
  /// или у пользователей совпадают UserId
  /// </summary>
  public ValueTask<bool> IsSameUserIdOrAdmin(IUser actor, IUser otherUser);

  /// <summary>
  /// Получить пользователя по Employee.
  /// </summary>
  /// <param name="employee">Объект сотрудника.</param>
  /// <returns>Объект пользователя.</returns>
  public Task<IUser?> GetByEmployee(IEmployee employee);
}

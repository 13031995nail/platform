namespace Tmobis.Business.Common.Sessions.Providers;
using Tmobis.Business.Common.Sessions.Models;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис работы с сессиями.
/// </summary>
public interface IUserSessionProvider
{
  /// <summary>
  /// Получение сессии по ID.
  /// </summary>
  /// <param name="id">Идентификатор сессии.</param>
  public Task<IUserSession?> GetById(Guid id);

  /// <summary>
  /// Остановка сессии.
  /// Без сохранения в хранилище.
  /// </summary>
  /// <param name="session">Сессия.</param>
  public void Expire(IUserSession session);

  /// <summary>
  /// Создание сессии для пользователя.
  /// </summary>
  /// <param name="user">Пользователь.</param>
  public IUserSession CreateByUser(IUser user);

  /// <summary>
  /// Очистка сессий всех пользователей.
  /// </summary>
  public Task ExpireAll();

  /// <summary>
  /// Получение сессии текущего пользователя.
  /// </summary>
  public Task<IUserSession?> GetCurrentSession();

  /// <summary>
  /// Получение ID сессии текущего пользователя.
  /// </summary>
  public Guid? GetCurrentSessionId();

  /// <summary>
  /// Предзагрузить сессии в кэш.
  /// </summary>
  // public Task PreloadSessions();
}

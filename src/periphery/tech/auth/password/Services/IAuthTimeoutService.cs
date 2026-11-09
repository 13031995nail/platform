namespace Tmobis.Auth.Password.Services;

using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Интерфейс сервиса таймаута авторизации.
/// </summary>
public interface IAuthTimeoutService
{
  /// <summary>
  /// Учет количества неудачных попыток авторизации.
  /// </summary>
  /// <param name="user">Авторизуемый пользователь.</param>
  /// <returns>Флаг, выводить ли сообщение о блокировке.</returns>
  bool IncreaseTriesNumber(IUser user);

  /// <summary>
  /// Заблокирована ли авторизация для учетной записи.
  /// </summary>
  /// <param name="user">Авторизуемый пользователь.</param>
  /// <returns>Флаг, заблокирована ли возможность входа.</returns>
  bool IsBlocked(IUser user);
}

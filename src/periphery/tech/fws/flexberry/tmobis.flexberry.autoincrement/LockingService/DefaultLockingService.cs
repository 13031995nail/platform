namespace Tmobis.Flexberry.Autoincrement.LockingService;

using ICSSoft.STORMNET.Business;
using Tmobis.Flexberry.Autoincrement.LockingAutoincrementService;

/// <summary>
/// Сервис блокировок счетчиков автоинкремента, использующий <see cref="LockService"/>.
/// </summary>
/// <seealso cref="ILockingService" />
public class DefaultLockingService(LockService lockService)
  : ILockingService
{
    /// <summary>
    /// Сервис блокировок.
    /// </summary>
    readonly LockService lockService = lockService;

    /// <summary>
    /// Снять блокировку по ключу счетчика.
    /// </summary>
    /// <param name="key">Ключ счетчика.</param>
    public void ClearLock(string key)
    {
        lockService.ClearLock(key);
    }

    /// <summary>
    /// Установить блокировку по ключу счетчика.
    /// </summary>
    /// <param name="key">Ключ счетчика.</param>
    /// <returns>
    /// Пустую строку если удалось установить блокировку, иначе - имя заблокировавшего пользователя.
    /// </returns>
    public string SetLock(string key)
    {
        return lockService.SetLock(key);
    }
}

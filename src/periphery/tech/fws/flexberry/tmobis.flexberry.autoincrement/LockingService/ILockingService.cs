namespace Tmobis.Flexberry.Autoincrement.LockingAutoincrementService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Интерфейс сервиса блокировок для счетчиков автоинкремента.
    /// </summary>
    public interface ILockingService
    {
        /// <summary>
        /// Установить блокировку по ключу счетчика.
        /// </summary>
        /// <param name="key">Ключ счетчика.</param>
        /// <returns>Пустую строку если удалось установить блокировку, иначе - имя заблокировавшего пользователя.</returns>
        string SetLock(string key);

        /// <summary>
        /// Снять блокировку по ключу счетчика.
        /// </summary>
        /// <param name="key">Ключ счетчика.</param>
        void ClearLock(string key);
    }
}

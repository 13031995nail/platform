namespace Tmobis.Flexberry.Autoincrement.LockingAutoincrementService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tmobis.Flexberry.Autoincrement.Objects;

    /// <summary>
    /// Интерфейс хранилища счетчиков автоинкремента.
    /// </summary>
    public interface ICounterStorage
    {
        /// <summary>
        /// Загрузить из хранилища счетчик автоинкремента по автоинкрементируемому объекту.
        /// </summary>
        /// <param name="autoincremented">Автоинкрементируемый объект.</param>
        /// <returns>Счетчик автоинкремента по автоинкрементируемому объекту.</returns>
        Counter LoadCounterByAutoincremented(IAutoincremented autoincremented);

        /// <summary>
        /// Загрузить из хранилища счетчик автоинкремента по ключу.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <returns>Счетчик автоинкремента по ключу.</returns>
        Counter LoadCounterByKey(string key);

        /// <summary>
        /// Создать и сохранить в хранилище счетчик автоинкремента по автоинкрементируемому объекту.
        /// </summary>
        /// <param name="autoincremented">Автоинкрементируемый объект.</param>
        /// <returns>Созданный счетчик автоинкремента по автоинкрементируемому объекту.</returns>
        Counter CreateCounterByAutoincremented(IAutoincremented autoincremented);

        /// <summary>
        /// Обновить счетчик автоинкремента в хранилище.
        /// </summary>
        /// <param name="updatedCounter">Обновляемый счетчик.</param>
        void UpdateCounter(Counter updatedCounter);

        /// <summary>
        /// Удалить счетчик автоинкремента в хранилище по ключу.
        /// </summary>
        /// <param name="key">Ключ.</param>
        void DeleteCounterByKey(string key);

        /// <summary>
        /// Сбросить счетчик автоинкремента.
        /// </summary>
        void ResetCounterAutoincremented();
  }
}

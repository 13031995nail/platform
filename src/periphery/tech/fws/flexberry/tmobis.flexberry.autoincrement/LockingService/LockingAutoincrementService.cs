namespace Tmobis.Flexberry.Autoincrement.LockingAutoincrementService
{
    using System;
    using System.Linq;
    using System.Threading;
    using ICSSoft.STORMNET.Business;
    using ICSSoft.STORMNET.Business.LINQProvider;
    using Tmobis.Flexberry.Autoincrement.Objects;
    
    /// <summary>
    /// Сервис автоинкремента, использующий сервис блокировок Flexberry для синхронизированного изменения счетчиков автоинкремента.
    /// </summary>
    /// <seealso cref="Tmobis.Flexberry.Autoincrement.IAutoincrementService" />
    public class LockingAutoincrementService : IAutoincrementService
    {
        /// <summary>
        /// Хранилище счетчиков.
        /// </summary>
        private ICounterStorage _counterStorage;

        /// <summary>
        /// Сервис блокировок.
        /// </summary>
        private ILockingService _lockingService;

        /// <summary>
        /// Количество попыток получить блокировку.
        /// </summary>
        private int _lockRetryCount = 5;

        /// <summary>
        /// Количество попыток получить блокировку.
        /// </summary>
        public int LockRetryCount
        {
            get { return _lockRetryCount; }
            set { _lockRetryCount = value; }
        }

        /// <summary>
        /// Интервал между повторными попытками в миллисекундах.
        /// </summary>
        private int _retryIntervalInMilliseconds = 100;

        /// <summary>
        /// Интервал между повторными попытками в миллисекундах.
        /// </summary>
        public int RetryIntervalInMilliseconds
        {
            get { return _retryIntervalInMilliseconds; }
            set { _retryIntervalInMilliseconds = value; }
        }

        /// <summary>
        /// Создает экземпляр <see cref="LockingAutoincrementService"/>.
        /// </summary>
        /// <param name="counterStorage">Хранилище счетчиков.</param>
        public LockingAutoincrementService(ICounterStorage counterStorage, ILockingService lockingService)
        {
            _counterStorage = counterStorage;
            _lockingService = lockingService;
        }

        /// <summary>
        /// Получить следующее значение счетчика автоинкремента для объекта.
        /// </summary>
        /// <param name="autoincremented">Объект, для которого получается следующее значение счетчика автоинкремента.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetNext(IAutoincremented autoincremented)
        {
            int result = -1;
            var counterKey = autoincremented.CounterKey;

            try
            {
                if (!TrySetLockByKey(counterKey))
                {
                    throw new InvalidOperationException($"Не удалось получить блокировку на счетчик с ключом {counterKey} и именем {autoincremented.CounterName}");
                }

                var counter = LoadOrCreateCounterByAutoincremented(autoincremented);

                int value = counter.Value;

                counter.Value += counter.Increment;
                _counterStorage.UpdateCounter(counter);

                result = value;
            }
            finally
            {
                _lockingService.ClearLock(counterKey);
            }

            return result;
        }

        /// <summary>
        /// Попытаться установить блокировку на ключ счетчика. 
        /// Осуществляется <see cref="LockRetryCount"/> попыток с интервалом <see cref="RetryIntervalInMilliseconds"/>.
        /// </summary>
        /// <param name="key">Ключ счетчика для блокировки.</param>
        /// <returns>Удалось ли создать блокировку.</returns>
        private bool TrySetLockByKey(string key)
        {
            int tryCount = LockRetryCount;
            var locked = _lockingService.SetLock(key);
            while (!string.IsNullOrEmpty(locked) && tryCount > 0)
            {
                tryCount--;
                Thread.Sleep(RetryIntervalInMilliseconds);
                locked = _lockingService.SetLock(key);
            }

            return string.IsNullOrEmpty(locked);
        }

        /// <summary>
        /// Загружает существующий или создает счетчик для заданного автоинкрементируемого объекта.
        /// </summary>
        /// <param name="autoincremented">Автоинкрементируемый объект.</param>
        /// <returns>Счетчик для заданного автоинкрементируемого объекта.</returns>
        private Counter LoadOrCreateCounterByAutoincremented(IAutoincremented autoincremented)
        {
            Counter counter = _counterStorage.LoadCounterByAutoincremented(autoincremented);
            if (counter == null)
            {
                counter = _counterStorage.CreateCounterByAutoincremented(autoincremented);
            }
            return counter;
        }

        /// <summary>
        /// Удалить счетчик автоинкремента из хранилища по ключу.
        /// </summary>
        /// <param name="key">Ключ счетчика автоинкремента.</param>
        public void DeleteCounterByKey(string key)
        {
            try
            {
                if (!TrySetLockByKey(key))
                {
                    throw new InvalidOperationException($"Не удалось получить блокировку на счетчик с ключом {key}");
                }
                _counterStorage.DeleteCounterByKey(key);
            }
            finally
            {
                _lockingService.ClearLock(key);
            }
        }
    }
}

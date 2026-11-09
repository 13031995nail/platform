namespace Tmobis.Flexberry.Autoincrement.LockingAutoincrementService
{
    using ICSSoft.STORMNET;
    using ICSSoft.STORMNET.Business;
    using ICSSoft.STORMNET.FunctionalLanguage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ICSSoft.STORMNET.Business.LINQProvider;
    using Tmobis.Flexberry.Autoincrement.Objects;

    /// <summary>
    /// Хранилище счетчиков автоинкремента в базе данных.
    /// </summary>
    /// <seealso cref="Autoincrement.LockingAutoincrementService.ICounterStorage" />
    public class DBCounterStorage : ICounterStorage
    {
        /// <summary>
        /// Сервис данных.
        /// </summary>
        IDataService _dataService;

        /// <summary>
        /// Создать экземпляр <see cref="DBCounterStorage"/>.
        /// </summary>
        /// <param name="dataService">Сервис данных</param>
        /// <exception cref="ArgumentNullException">dataService</exception>
        public DBCounterStorage(IDataService dataService)
        {
            if (dataService == null)
            {
                throw new ArgumentNullException($"{nameof(dataService)}");
            }

            _dataService = dataService;
        }

        /// <summary>
        /// Создать и сохранить в хранилище счетчик автоинкремента по автоинкрементируемому объекту.
        /// </summary>
        /// <param name="autoincremented">Автоинкрементируемый объект.</param>
        /// <returns>
        /// Созданный счетчик автоинкремента по автоинкрементируемому объекту.
        /// </returns>
        public Counter CreateCounterByAutoincremented(IAutoincremented autoincremented)
        {
            var newCounter = new Counter()
            {
                Key = autoincremented.CounterKey,
                Name = autoincremented.CounterName,                
                Increment = 1,
                Value = 1
            };
            _dataService.UpdateObject(newCounter);

            return newCounter;
        }        

        /// <summary>
        /// Загрузить из хранилища счетчик автоинкремента по автоинкрементируемому объекту.
        /// </summary>
        /// <param name="autoincremented">Автоинкрементируемый объект.</param>
        /// <returns>
        /// Счетчик автоинкремента по автоинкрементируемому объекту.
        /// </returns>
        public Counter LoadCounterByAutoincremented(IAutoincremented autoincremented)
        {
            return LoadCounterByKey(autoincremented.CounterKey);
        }

        /// <summary>
        /// Сбросить счетчик автоинкремента.
        /// </summary>
        public void ResetCounterAutoincremented()
        {
          var counters = _dataService.Query<Counter>(Counter.Views.CounterE).ToList();

          foreach (var counter in counters)
          {
            counter.Value = 1;
            UpdateCounter(counter);
          }
        }

        /// <summary>
        /// Загрузить из хранилища счетчик автоинкремента по ключу.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <returns>
        /// Счетчик автоинкремента по ключу.
        /// </returns>
        /// <exception cref="Exception"></exception>
        public Counter LoadCounterByKey(string key)
        {
            var lcs = LoadingCustomizationStruct.GetSimpleStruct(typeof(Counter), Counter.Views.CounterE);
            lcs.LimitFunction = FunctionBuilder.BuildEquals<Counter>(x => x.Key,key);
            lcs.ReturnTop = 2;
            var counters = _dataService.LoadObjects(lcs);

            if (counters.Length > 1)
            {
                throw new ArgumentException($"Обнаружено более одного счетчика с ключом {key}");
            }

            return counters.FirstOrDefault() as Counter;
        }

        /// <summary>
        /// Обновить счетчик автоинкремента в хранилище.
        /// </summary>
        /// <param name="updatedCounter">Обновляемый счетчик.</param>
        public void UpdateCounter(Counter updatedCounter)
        {
            _dataService.UpdateObject(updatedCounter);
        }


        /// <summary>
        /// Удалить счетчик автоинкремента в хранилище по ключу.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void DeleteCounterByKey(string key)
        {
            var counterToDelete = LoadCounterByKey(key);
            if (counterToDelete == null)
            {
                throw new KeyNotFoundException($"Не найден счетчик с ключом {key}");
            }
            counterToDelete.SetStatus(ObjectStatus.Deleted);
            _dataService.UpdateObject(counterToDelete);
        }
    }
}

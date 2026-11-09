namespace Tmobis.Frameworks.Flexberry.Repositories;

using ICSSoft.STORMNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Хранит данные, общие для операций транзакционного FlexberryRepository внутри одного DI-scope.
/// Содержит общий DataObjectCache и список объектов, которые должны быть переданы в Commit.
/// Не является отдельным трекером изменений, а использует состояние самих DataObject.
/// </summary>
public class FlexberryRepositoryContext : IDisposable
{
    private readonly Dictionary<(Type Type, object PrimaryKey), DataObject> pendingByKey = [];
    private readonly HashSet<DataObject> pendingWithoutKey =
      new(ReferenceEqualityComparer.Instance);

    public FlexberryRepositoryContext()
    {
        DataObjectCache = new DataObjectCache();
        // Кэш создаётся на весь scope репозитория, чтобы чтение и сохранение работали с одними экземплярами DataObject.
        DataObjectCache.StartCaching(false);
    }

    /// <summary>
    /// Общий кэш объектов данных в рамках scope репозитория.
    /// </summary>
    public DataObjectCache DataObjectCache { get; }

    /// <summary>
    /// Объекты, явно переданные в Create/Update/Delete и ожидающие отправки в UpdateObjects при Commit.
    /// </summary>
    public IReadOnlyCollection<DataObject> PendingObjects
    => pendingByKey.Values
      .Concat(pendingWithoutKey)
      .ToList();

    /// <summary>
    /// Возвращает объект из DataObjectCache по типу и первичному ключу.
    /// </summary>
    public DataObject? GetFromCache(Type dataObjectType, object primaryKey)
    {
        return DataObjectCache.GetLivingDataObject(dataObjectType, primaryKey) as DataObject;
    }

    /// <summary>
    /// Добавляет объект в DataObjectCache или возвращает уже существующий экземпляр с тем же ключом.
    /// </summary>
    public DataObject AddToCacheOrGetCached(DataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        var primaryKey = dataObject.__PrimaryKey;
        if (IsEmptyPrimaryKey(primaryKey))
        {
            DataObjectCache.AddDataObject(dataObject);
            return dataObject;
        }

        var cached = GetFromCache(dataObject.GetType(), primaryKey!);
        if (cached != null)
        {
            return cached;
        }

        DataObjectCache.AddDataObject(dataObject);
        return dataObject;
    }

    /// <summary>
    /// Добавляет объект данных в список на сохранение, не дублируя его по типу и первичному ключу.
    /// Для объектов без ключа использует сравнение по ссылке.
    /// </summary>
    public void AddToUpdate(DataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        var primaryKey = dataObject.__PrimaryKey;
        if (!IsEmptyPrimaryKey(primaryKey))
        {
            pendingByKey[(dataObject.GetType(), primaryKey!)] = dataObject;
            return;
        }

        pendingWithoutKey.Add(dataObject);
    }

    /// <summary>
    /// Очищает только список объектов на сохранение.
    /// DataObjectCache продолжает жить до завершения текущего scope.
    /// </summary>
    public void ClearPendingObjects()
    {
        pendingByKey.Clear();
        pendingWithoutKey.Clear();
    }

    /// <summary>
    /// Завершает использование DataObjectCache для текущего scope.
    /// </summary>
    public void Dispose()
    {
        DataObjectCache.StopCaching();
    }

    private static bool IsEmptyPrimaryKey(object? primaryKey)
    => primaryKey is null || primaryKey is DBNull || Equals(primaryKey, Guid.Empty);

    private sealed class ReferenceEqualityComparer : IEqualityComparer<DataObject>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public bool Equals(DataObject? x, DataObject? y)
        => ReferenceEquals(x, y);

        public int GetHashCode(DataObject obj)
        => RuntimeHelpers.GetHashCode(obj);
    }
}

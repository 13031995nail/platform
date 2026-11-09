namespace Tmobis.Frameworks.Flexberry.Repositories;

using ICSSoft.STORMNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Оборачивает отложенный запрос из raw-репозитория и согласует материализованные DataObject с DataObjectCache текущей области.
/// Нужен для Query, чтобы результат не обходил кэш FlexberryRepositoryContext.
/// </summary>
internal sealed class DataObjectCacheQueryable<T> : IOrderedQueryable<T>
{
    private readonly IQueryable<T> source;
    private readonly FlexberryRepositoryContext context;

    /// <summary>
    /// Принимает исходный запрос и контекст репозитория; Provider создаётся как обёртка над исходным IQueryProvider.
    /// </summary>
    public DataObjectCacheQueryable(IQueryable<T> source, FlexberryRepositoryContext context)
    {
        this.source = source;
        this.context = context;
        Provider = new DataObjectCacheQueryProvider(source.Provider, context);
    }

    /// <summary>
    /// Тип элемента исходного запроса.
    /// </summary>
    public Type ElementType => typeof(T);

    /// <summary>
    /// Выражение исходного запроса без изменений.
    /// </summary>
    public Expression Expression => source.Expression;

    /// <summary>
    /// Поставщик запросов, который делегирует выполнение исходному provider-у и оборачивает новые IQueryable.
    /// </summary>
    public IQueryProvider Provider { get; }

    /// <summary>
    /// Перечисляет исходный запрос и при выдаче каждого элемента согласует DataObject с DataObjectCache.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in source)
        {
            yield return Map(item, context);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

    // Согласование только DataObject с кэшем; проекции и другие значения не меняются.
    private static T Map(T item, FlexberryRepositoryContext context)
    {
        if (item is DataObject dataObject)
        {
            var cached = context.AddToCacheOrGetCached(dataObject);
            return cached is T typed ? typed : item;
        }

        return item;
    }

    /// <summary>
    /// Делегирует построение и выполнение выражений исходному IQueryProvider, повторно оборачивает созданные IQueryable
    /// и согласует одиночный DataObject с DataObjectCache при Execute.
    /// </summary>
    private sealed class DataObjectCacheQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider sourceProvider;
        private readonly FlexberryRepositoryContext context;

        public DataObjectCacheQueryProvider(IQueryProvider sourceProvider, FlexberryRepositoryContext context)
        {
            this.sourceProvider = sourceProvider;
            this.context = context;
        }

        /// <summary>
        /// Создаёт новый запрос через исходный provider и возвращает его в обёртке с согласованием кэша.
        /// </summary>
        public IQueryable CreateQuery(Expression expression)
        {
            var query = sourceProvider.CreateQuery(expression);
            var queryableType = typeof(DataObjectCacheQueryable<>).MakeGenericType(query.ElementType);
            return (IQueryable)(Activator.CreateInstance(queryableType, query, context)
              ?? throw new InvalidOperationException("Не удалось создать обёртку запроса с DataObjectCache."));
        }

        /// <summary>
        /// Создаёт типизированный запрос через исходный provider и возвращает его в обёртке с согласованием кэша.
        /// </summary>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var query = sourceProvider.CreateQuery<TElement>(expression);
            return new DataObjectCacheQueryable<TElement>(query, context);
        }

        /// <summary>
        /// Выполняет scalar или одиночный результат через исходный provider и согласует DataObject, если он получен.
        /// </summary>
        public object? Execute(Expression expression)
        {
            var result = sourceProvider.Execute(expression);
            return MapObject(result);
        }

        /// <summary>
        /// Выполняет типизированный scalar или одиночный результат через исходный provider и согласует DataObject, если он получен.
        /// </summary>
        public TResult Execute<TResult>(Expression expression)
        {
            var result = sourceProvider.Execute<TResult>(expression);
            var mapped = MapObject(result);
            return mapped is TResult typed ? typed : result;
        }

        private object? MapObject(object? item)
        {
            if (item is DataObject dataObject)
            {
                return context.AddToCacheOrGetCached(dataObject);
            }

            return item;
        }
    }
}

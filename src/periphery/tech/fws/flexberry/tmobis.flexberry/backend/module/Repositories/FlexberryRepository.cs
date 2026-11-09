namespace Tmobis.Frameworks.Flexberry.Repositories;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

/// <summary>
/// Реализует IRepository для Flexberry ORM с отложенным сохранением изменений.
/// Использует DataObjectCache и список на сохранение из FlexberryRepositoryContext.
/// Create/Update/Delete только накапливают изменения, а Commit передаёт их в IDataService.UpdateObjects.
/// </summary>
public class FlexberryRepository(
  IServiceProvider provider,
  IEntityMaterializer materializer,
  IDataService dataService,
  FlexberryRepositoryContext context,
  string? entityRepositoryKey = null)
  : IAtomicRepository, IUnitOfWork
{

    private readonly Dictionary<Type, object> repositories = [];

    /// <summary>
    /// Возвращает сущность по идентификатору: сначала из DataObjectCache, затем через IDataService с тем же DataObjectCache.
    /// Если объект уже есть в кэше, возвращает кэшированный экземпляр.
    /// </summary>
    public Task<T?> GetById<T>(
      Guid id,
      IEntityView? view = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
      cancellationToken.ThrowIfCancellationRequested();
      return Task.FromResult(GetByIdInternal<T>(id, view));
    }

    private T? GetByIdInternal<T>(Guid id, IEntityView? view = null)
      where T : class, IEntity
    {
        var storedType = materializer.ResolveStoredEntityType<T>();
        if (context.GetFromCache(storedType, id) is T cached)
        {
            if (view != null && cached is DataObject cachedDataObject)
            {
                LoadDataObjectWithoutClearing(storedType, cachedDataObject, view);
            }

            return cached;
        }

        var entity = GetRepository<T>().MakeEntity(id);
        if (entity is not DataObject dataObject)
        {
            return null;
        }

        var flexberryView = ResolveFlexberryView(storedType, view);
        try
        {
            dataService.LoadObject(flexberryView, dataObject, context.DataObjectCache);
        }
        catch (Exception ex) when (IsObjectNotFoundException(ex))
        {
            return null;
        }

        return MapReadEntity(entity);
    }

    /// <summary>
    /// Возвращает сущности по списку идентификаторов и согласует материализованные результаты с DataObjectCache.
    /// Само чтение не добавляет объекты в список на сохранение.
    /// </summary>
    public async Task<List<T>> GetByIds<T>(
      IReadOnlyCollection<Guid> ids,
      IEntityView? view = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
      cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(ids);

        var distinctIds = ids.Distinct().ToList();
        var byId = new Dictionary<Guid, T>();
        var missingIds = new List<Guid>();
        var storedType = materializer.ResolveStoredEntityType<T>();

        foreach (var id in distinctIds)
        {
            if (context.GetFromCache(storedType, id) is T cached)
            {
                byId[id] = cached;
            }
            else
            {
                missingIds.Add(id);
            }
        }

        if (missingIds.Count > 0)
        {
            var loaded = await GetRepository<T>().GetByIds(missingIds, view, cancellationToken);
            foreach (var entity in loaded)
            {
                if (!byId.ContainsKey(entity.Id))
                {
                    byId[entity.Id] = MapReadEntity(entity) ?? entity;
                }
            }
        }

        return distinctIds
          .Where(byId.ContainsKey)
          .Select(id => byId[id])
          .ToList();
    }

    /// <summary>
    /// Выполняет поиск и согласует полученные объекты с DataObjectCache без добавления в список на сохранение.
    /// </summary>
    public async Task<List<T>> Find<T>(
      Expression<Func<T, bool>>? predicate = null,
      IEntityView? view = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
      int? skip = null,
      int? take = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
        var result = await GetRepository<T>().Find(
          predicate, view, orderBy, skip, take, cancellationToken);
        return MapReadEntities(result);
    }

    /// <summary>
    /// Возвращает одну сущность по условию и согласует её с DataObjectCache.
    /// </summary>
    public async Task<T?> FirstOrDefault<T>(
      Expression<Func<T, bool>> predicate,
      IEntityView? view = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
        var result = await GetRepository<T>().FirstOrDefault(predicate, view, cancellationToken);
        return MapReadEntity(result);
    }

    /// <summary>
    /// Возвращает список сущностей и согласует материализованные экземпляры с DataObjectCache.
    /// Чтение не добавляет объекты в список на сохранение.
    /// </summary>
    public async Task<List<T>> All<T>(
      IEntityView? view = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
      int? skip = null,
      int? take = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
        var result = await GetRepository<T>().All(view, orderBy, skip, take, cancellationToken);
        return MapReadEntities(result);
    }

    /// <summary>
    /// Проверяет существование хотя бы одной сущности, подходящей под условие.
    /// </summary>
    public Task<bool> Any<T>(
      Expression<Func<T, bool>> predicate,
      IEntityView? view = null,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
      => GetRepository<T>().Any(predicate, view, cancellationToken);


    /// <summary>
    /// Возвращает количество сущностей, подходящих под условие.
    /// </summary>
    public Task<int> Count<T>(
      Expression<Func<T, bool>>? predicate = null,
      IEntityView? view = null,
      CancellationToken cancellationToken = default)
    where T : class, IEntity
    => GetRepository<T>().Count(predicate, view, cancellationToken);

    /// <summary>
    /// Возвращает отложенный Query для указанной сущности.
    /// </summary>
    public IQueryable<T> Query<T>(IEntityView? view = null)
      where T : class, IEntity
    {
        // Запрос остаётся отложенным: выражение выполняет entity-репозиторий,
        // а материализованные DataObject согласуются с DataObjectCache при получении результата.
        return new DataObjectCacheQueryable<T>(GetRepository<T>().Query(view), context);
    }

    /// <summary>
    /// Создаёт сущность через entity-репозиторий, добавляет её в DataObjectCache и не включает в список на сохранение
    /// до явного вызова Create/Update/Delete.
    /// </summary>
    public T MakeEntity<T>(Guid? id = null) where T : class, IEntity
    {
        var entity = GetRepository<T>().MakeEntity(id);
        if (entity is DataObject dataObject)
        {
            var cached = context.AddToCacheOrGetCached(dataObject);
            return cached as T ?? entity;
        }

        return entity;
    }

    /// <summary>
    /// Догружает сущность по указанному представлению через общий DataObjectCache.
    /// </summary>
    public Task SubLoad<T>(
      T entity,
      IEntityView view,
      CancellationToken cancellationToken = default)
      where T : class, IEntity
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(view);

        if (entity is not DataObject dataObject)
        {
            throw new InvalidOperationException(
              $"Сущность типа \"{entity.GetType().FullName}\" не является DataObject и не может быть догружена через FlexberryRepository.");
        }

        var storedType = materializer.ResolveStoredEntityType<T>();
        LoadDataObjectWithoutClearing(storedType, dataObject, view);
        context.AddToCacheOrGetCached(dataObject);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Подготавливает изменённую сущность к сохранению и добавляет её в список на сохранение.
    /// Не вызывает UpdateObject/UpdateObjects сразу.
    /// </summary>
    public void Update(IEntity model)
    {
        var dataObject = EnsureDataObject(model);
        PrepareAlteredProperties(dataObject);

        var cached = context.AddToCacheOrGetCached(dataObject);
        if (!ReferenceEquals(cached, dataObject))
        {
            throw CreateDuplicateInstanceException(dataObject);
        }

        context.AddToUpdate(dataObject);
    }

    public void Update(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        foreach (var model in models)
            Update(model);
    }

    /// <summary>
    /// Помечает сущность как созданную и добавляет её в список на сохранение без немедленной записи в хранилище.
    /// </summary>
    public void Create(IEntity model)
    {
        var dataObject = EnsureDataObject(model);
        dataObject.SetStatus(ObjectStatus.Created);

        var cached = context.AddToCacheOrGetCached(dataObject);
        if (!ReferenceEquals(cached, dataObject))
        {
            throw CreateDuplicateInstanceException(dataObject);
        }

        context.AddToUpdate(dataObject);
    }

    public void Create(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        foreach (var model in models)
            Create(model);
    }

    /// <summary>
    /// Помечает сущность как удалённую и добавляет её в список на сохранение без немедленной записи в хранилище.
    /// </summary>
    public void Delete(IEntity model)
    {
        var dataObject = EnsureDataObject(model);
        dataObject.SetStatus(ObjectStatus.Deleted);

        var cached = context.AddToCacheOrGetCached(dataObject);
        if (!ReferenceEquals(cached, dataObject))
        {
            throw CreateDuplicateInstanceException(dataObject);
        }

        context.AddToUpdate(dataObject);
    }

    public void Delete<T>(IEnumerable<T> models)
        where T : IEntity
    {
        ArgumentNullException.ThrowIfNull(models);
        foreach (var model in models)
            Delete(model);
    }

    public void Delete(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        foreach (var model in models)
            Delete(model);
    }

    public void Delete<T>(Guid id) where T : class, IEntity
        => GetRepository<T>().DeleteById(id);

    public void Delete<T>(IReadOnlyCollection<Guid> ids) where T : class, IEntity
        => GetRepository<T>().DeleteByIds(ids);

    /// <summary>
    /// Фиксирует накопленные изменения через Flexberry ORM.
    /// Передаёт в UpdateObjects только изменённые DataObject и после успешного сохранения очищает
    /// список на сохранение, сохраняя DataObjectCache до конца scope.
    /// </summary>
    public Task Commit(CancellationToken cancellationToken = default)
        => CommitInternal(alwaysThrowException: false, cancellationToken);

    public Task CommitAtomically()
        => CommitInternal(alwaysThrowException: true, CancellationToken.None);

    private Task CommitInternal(bool alwaysThrowException, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // UnAltered не нужно отправлять в UpdateObjects, чтобы исключить лишние операции.
        var objects = context.PendingObjects
          .Where(dataObject => dataObject.GetStatus() != ObjectStatus.UnAltered)
          .ToArray();

        if (objects.Length == 0)
        {
            return Task.CompletedTask;
        }

        // Бизнес-серверы и объекты, которые они возвращают через toUpdate,
        // обрабатываются внутри Flexberry ORM при UpdateObjects.
        if (alwaysThrowException)
        {
            if (dataService is not SQLDataService sqlDataService)
                throw new NotSupportedException("Сервис данных не поддерживает атомарное сохранение.");
            sqlDataService.UpdateObjects(ref objects, context.DataObjectCache, alwaysThrowException: true);
        }
        else
        {
            dataService.UpdateObjects(ref objects, context.DataObjectCache);
        }
        context.ClearPendingObjects();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Возвращает raw-репозиторий для указанного интерфейса сущности.
    /// </summary>
    protected virtual IEntityRepository<T> GetRepository<T>() where T : class, IEntity
    {
        var type = typeof(T);
        if (!repositories.TryGetValue(type, out var repository))
        {
            var storedType = materializer.ResolveStoredEntityType<T>();
            var repositoryType = typeof(IEntityRepository<,>).MakeGenericType(type, storedType);
            repository = entityRepositoryKey is null
              ? provider.GetRequiredService(repositoryType)
              : provider.GetRequiredKeyedService(repositoryType, entityRepositoryKey);
            repositories.Add(type, repository);
        }

        return repository as IEntityRepository<T>
               ?? throw new InvalidOperationException("Не удалось получить репозиторий");
    }

    /// <summary>
    /// Определяет исключение чтения, соответствующее сценарию отсутствия объекта в хранилище.
    /// </summary>
    private bool IsObjectNotFoundException(Exception exception)
    {
        // TODO: Уточнить точный тип исключения Flexberry при отсутствии объекта и заменить проверку по имени типа.
        var typeName = exception.GetType().Name;
        return typeName.Contains("NotFound", StringComparison.Ordinal)
          || typeName.Contains("CantFind", StringComparison.Ordinal);
    }

    /// <summary>
    /// Возвращает представление Flexberry для чтения, используя имя из IEntityView или edit-view по умолчанию.
    /// </summary>
    private static View ResolveFlexberryView(Type storedType, IEntityView? view)
    => FlexberryViewResolver.Resolve(storedType, view);

    /// <summary>
    /// Догружает DataObject по представлению без очистки уже загруженных данных.
    /// </summary>
    private void LoadDataObjectWithoutClearing(Type storedType, DataObject dataObject, IEntityView view)
    {
        var flexberryView = ResolveFlexberryView(storedType, view);
        dataService.LoadObject(
          flexberryView,
          dataObject,
          clearDataObject: false,
          checkExistingObject: true,
          context.DataObjectCache);
    }

    /// <summary>
    /// Согласует прочитанный объект с DataObjectCache.
    /// </summary>
    private T? MapReadEntity<T>(T? entity) where T : class
    {
        if (entity == null)
        {
            return null;
        }

        if (entity is not DataObject dataObject)
        {
            return entity;
        }

        var cached = context.AddToCacheOrGetCached(dataObject);
        return cached as T ?? entity;
    }

    /// <summary>
    /// Согласует список прочитанных объектов с DataObjectCache.
    /// </summary>
    private List<T> MapReadEntities<T>(IEnumerable<T> entities) where T : class
    {
        return entities
          .Select(entity => MapReadEntity(entity) ?? entity)
          .ToList();
    }

    /// <summary>
    /// Создаёт исключение для сценария двух разных экземпляров одной сущности в пределах одного scope.
    /// </summary>
    private static InvalidOperationException CreateDuplicateInstanceException(DataObject dataObject)
    => new(
      $"В текущем scope уже есть другой экземпляр сущности типа \"{dataObject.GetType().FullName}\" с ключом \"{dataObject.__PrimaryKey}\". Нельзя сохранить разные экземпляры одной сущности.");

    /// <summary>
    /// Проверяет, что сущность поддерживает сохранение через FlexberryRepository как DataObject.
    /// </summary>
    private static DataObject EnsureDataObject(IEntity model)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (model is DataObject dataObject)
        {
            return dataObject;
        }

        throw new InvalidOperationException(
          $"Сущность типа \"{model.GetType().FullName}\" не является DataObject и не может быть сохранена через FlexberryRepository.");
    }

    /// <summary>
    /// Добавляет изменённые свойства в список загруженных, чтобы Flexberry ORM корректно сохранила
    /// частично загруженный объект данных.
    /// </summary>
    private static void PrepareAlteredProperties(DataObject dataObject)
    {
        foreach (var alteredProp in dataObject.GetAlteredPropertyNames())
        {
            if (!dataObject.CheckLoadedProperty(alteredProp))
            {
                dataObject.AddLoadedProperties(alteredProp);
            }
        }
    }
}

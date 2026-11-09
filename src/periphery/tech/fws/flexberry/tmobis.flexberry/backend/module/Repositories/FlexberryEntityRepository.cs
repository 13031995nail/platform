namespace Tmobis.Frameworks.Flexberry.Repositories;

using System.Linq.Expressions;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;
using Tmobis.Frameworks.Flexberry.Linq;

public class FlexberryEntityRepository<TI, T>(
    IDataService dataService,
    FlexberryRepositoryContext context)
    : IEntityRepository<TI, T>
    where TI : class, IEntity
    where T : DataObject, IStoredEntity, TI, new()
{
    public virtual Task<bool> Any(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Query(view).Any(predicate));
    }

    public virtual Task<int> Count(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = Query(view);
        var count = predicate == null
            ? query.Count()
            : query.Count(predicate);

        return Task.FromResult(count);
    }

    public virtual Task<List<TI>> Find(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Query(view)
            .ApplyListQuery(predicate, orderBy, skip, take)
            .ToList());
    }

    public virtual Task<TI?> FirstOrDefault(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Query(view).FirstOrDefault(predicate));
    }

    public virtual Task<TI?> GetById(
        Guid id,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = Query(view);
        var entity = query.FirstOrDefault(item => item.Id.Equals(id));
        return Task.FromResult(entity);
    }

    public virtual Task<List<TI>> GetByIds(
        IReadOnlyCollection<Guid> ids,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(ids);

        var idsObj = ids.Cast<object>().ToList();
        var query = Query(view);
        var entities = query.Where(
            entity => idsObj.Contains(entity.Id)).ToList();
        return Task.FromResult(entities);
    }

    public virtual IQueryable<TI> Query(IEntityView? view = null)
        => dataService.QueryExt<T>(GetFlexberryView(view)).Cast<TI>();

    public Task<List<TI>> All(
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        => Find(null, view, orderBy, skip, take, cancellationToken);

    public virtual TI MakeEntity(Guid? id = null)
    {
        var primaryKey = id ?? Guid.NewGuid();
        return PKHelper.CreateDataObject<T>(primaryKey);
    }

    public void DeleteById(Guid id)
    {
        var cached = context.GetFromCache(typeof(T), id);
        var dataObject = cached ?? PKHelper.CreateDataObject<T>(id);
        if (cached == null)
        {
            var loaded = Query(null).FirstOrDefault(entity => entity.Id.Equals(id));
            if (loaded is not DataObject loadedObject)
            {
                return;
            }

            dataObject = loadedObject;
        }

        dataObject.SetStatus(ObjectStatus.Deleted);
        var stored = context.AddToCacheOrGetCached(dataObject);
        if (!ReferenceEquals(stored, dataObject))
        {
            stored.SetStatus(ObjectStatus.Deleted);
            dataObject = stored;
        }

        context.AddToUpdate(dataObject);
    }

    public void DeleteByIds(IReadOnlyCollection<Guid> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        foreach (var id in ids)
            DeleteById(id);
    }

    protected virtual View GetFlexberryView(IEntityView? view = null)
        => FlexberryViewResolver.Resolve(typeof(T), view);
}

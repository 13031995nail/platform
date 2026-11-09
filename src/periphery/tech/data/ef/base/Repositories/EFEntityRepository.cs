namespace Tmobis.Data.EF.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class EFEntityRepository<TI, T>(
    IDbContext databaseContext,
    IEFEntityAccessPolicy<TI, T> accessPolicy,
    IEFEntityViewLoader entityViewLoader)
    : IEntityRepository<TI, T>
    where TI : class, IEntity
    where T : BusinessEntity, TI, new()
{
    private readonly DbSet<T> dbSet =
        databaseContext.GetTable<T>();

    protected EFEntityRepository(
        IDbContext databaseContext,
        IEFEntityAccessPolicy<TI, T> accessPolicy,
        IEFEntityViewLoader entityViewLoader,
        bool _)
        : this(databaseContext, accessPolicy, entityViewLoader)
    {
    }

    public virtual async Task<TI?> GetById(
        Guid id,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);

        if (TryGetFromLocalCache(id, out var cachedEntity))
        {
            if (!await accessPolicy
                    .IsAllowedLocalAsync(cachedEntity, cancellationToken))
            {
                return default;
            }

            if (view != null && view.References.Any())
            {
                await entityViewLoader
                    .LoadAsync(cachedEntity, view, cancellationToken);
            }

            return cachedEntity;
        }

        var query = BuildQuery(view);

        return await query
            .FirstOrDefaultAsync(
                entity => entity.Id == id,
                cancellationToken);
    }

    private bool TryGetFromLocalCache(
        Guid id,
        out T? entity)
    {
        entity =
            dbSet.Local.FirstOrDefault(candidate =>
                candidate.Id == id);

        return entity != null;
    }

    public virtual async Task<List<TI>> GetByIds(
        IReadOnlyCollection<Guid> ids,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
    {
        if (ids == null || ids.Count == 0)
            return [];

        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);

        var distinctIds =
            ids.Distinct().ToList();

        var byId =
            new Dictionary<Guid, TI>(distinctIds.Count);

        var missingIds =
            new List<Guid>();

        var localEntities =
            new Dictionary<Guid, T>();

        foreach (var id in distinctIds)
        {
            if (TryGetFromLocalCache(
                    id,
                    out var cachedEntity))
            {
                localEntities[id] = cachedEntity;
            }
            else
            {
                missingIds.Add(id);
            }
        }

        if (localEntities.Count > 0)
        {
            var allowedLocalIds =
                await accessPolicy
                    .FilterAllowedLocalIdsAsync(
                        localEntities.Values.ToList(),
                        cancellationToken);

            foreach (var (id, entity) in localEntities)
            {
                if (!allowedLocalIds.Contains(id))
                    continue;

                if (view != null &&
                    view.References.Any())
                {
                    await entityViewLoader
                        .LoadAsync(entity, view, cancellationToken);
                }

                byId[id] = entity;
            }
        }

        if (missingIds.Count > 0)
        {
            var query = BuildQuery(view);

            var dbEntities = await query
                .Where(entity =>
                    missingIds.Contains(entity.Id))
                .ToListAsync(cancellationToken);

            foreach (var entity in dbEntities)
                byId.TryAdd(entity.Id, entity);
        }

        return distinctIds
            .Where(byId.ContainsKey)
            .Select(id => byId[id])
            .ToList();
    }

    public virtual Task<List<TI>> All(
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        => Find(
            predicate: null,
            view,
            orderBy,
            skip,
            take,
            cancellationToken);

    public virtual Task<bool> Any(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        => AnyInternal(predicate, view, cancellationToken);

    private async Task<bool> AnyInternal(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view,
        CancellationToken cancellationToken)
    {
        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);
        var query = BuildQuery(view);
        return await query.AnyAsync(predicate, cancellationToken);
    }

    public virtual Task<int> Count(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        => CountInternal(predicate, view, cancellationToken);

    private async Task<int> CountInternal(
        Expression<Func<TI, bool>>? predicate,
        IEntityView? view,
        CancellationToken cancellationToken)
    {
        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);
        var query = BuildQuery(view);

        return predicate == null
            ? await query.CountAsync(cancellationToken)
            : await query.CountAsync(predicate, cancellationToken);
    }

    public virtual TI MakeEntity(Guid? id = null)
        => MakeEntityInternal(id ?? Guid.NewGuid());

    public virtual void DeleteById(Guid id)
        => DeleteByIdInternal(id);

    public virtual void DeleteByIds(IReadOnlyCollection<Guid> ids)
    {
        ArgumentNullException.ThrowIfNull(ids);

        foreach (var id in ids)
            DeleteByIdInternal(id);
    }

    protected virtual void DeleteByIdInternal(Guid id)
    {
        var entry = dbSet.Local.FindEntry(id);

        if (entry == null)
        {
            var entity = MakeEntityInternal(id);
            dbSet.Attach(entity);
            dbSet.Remove(entity);
        }
        else
        {
            entry.State = EntityState.Deleted;
        }
    }

    public virtual async Task<List<TI>> Find(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);

        var query = BuildQuery(view)
            .ApplyListQuery(predicate, orderBy, skip, take);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual Task<TI?> FirstOrDefault(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        => FirstOrDefaultInternal(predicate, view, cancellationToken);

    private async Task<TI?> FirstOrDefaultInternal(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view,
        CancellationToken cancellationToken)
    {
        await accessPolicy.EnsureCurrentTownAsync(cancellationToken);
        var query = BuildQuery(view);
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public IQueryable<TI> Query(IEntityView? view = null)
        => BuildQuery(view);

    private IQueryable<TI> BuildQuery(IEntityView? view = null)
    {
        IQueryable<T> query = accessPolicy.Apply(dbSet);
        return ApplyView(query, view);
    }

    private static IQueryable<T> ApplyView(
        IQueryable<T> query,
        IEntityView? view)
    {
        if (view == null ||
            !view.References.Any())
        {
            return query;
        }

        foreach (var reference in view.References)
        {
            query = query.Include(reference);
        }

        return query;
    }

    protected virtual T MakeEntityInternal(Guid? id = null)
    {
        var newEntity = new T();

        if (id.HasValue)
            newEntity.Id = id.Value;

        return newEntity;
    }
}

public class EFUnrestrictedEntityRepository<TI, T>(
    IDbContext databaseContext,
    IEFEntityViewLoader entityViewLoader)
    : EFEntityRepository<TI, T>(
        databaseContext,
        EFNoAccessPolicy<TI, T>.Instance,
        entityViewLoader,
        true)
    where TI : class, IEntity
    where T : BusinessEntity, TI, new()
{
}

public class EFTownLimitedRepository(
    IEntityRepositoryResolver entityRepositoryResolver,
    IEFEntityViewLoader entityViewLoader,
    IEFEntityChangeProcessor entityChangeProcessor,
    Lazy<Audit.IAuditWriter> auditWriterLazy,
    IDbContext dbContext)
    : EFRepository(
        entityRepositoryResolver,
        entityViewLoader,
        entityChangeProcessor,
        auditWriterLazy,
        dbContext,
        RepositoryKeys.TownLimited);

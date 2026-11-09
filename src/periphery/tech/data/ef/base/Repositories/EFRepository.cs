namespace Tmobis.Data.EF.Repositories;

using System.Linq.Expressions;
using Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class EFRepository(
    IEntityRepositoryResolver entityRepositoryResolver,
    IEFEntityViewLoader entityViewLoader,
    IEFEntityChangeProcessor entityChangeProcessor,
    Lazy<IAuditWriter> auditWriterLazy,
    IDbContext dbContext,
    string? entityRepositoryKey = null)
    : IRepository, IUnitOfWork
{
    public Task<T?> GetById<T>(
        Guid id,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().GetById(id, view, cancellationToken);

    public Task<List<T>> GetByIds<T>(
        IReadOnlyCollection<Guid> ids,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().GetByIds(ids, view, cancellationToken);

    public Task<List<T>> Find<T>(
        Expression<Func<T, bool>>? predicate = null,
        IEntityView? view = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().Find(
            predicate,
            view,
            orderBy,
            skip,
            take,
            cancellationToken);

    public Task<T?> FirstOrDefault<T>(
        Expression<Func<T, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().FirstOrDefault(predicate, view, cancellationToken);

    public Task<List<T>> All<T>(
        IEntityView? view = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().All(view, orderBy, skip, take, cancellationToken);

    public Task<bool> Any<T>(
        Expression<Func<T, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().Any(predicate, view, cancellationToken);

    public Task<int> Count<T>(
        Expression<Func<T, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => GetRepository<T>().Count(predicate, view, cancellationToken);

    public IQueryable<T> Query<T>(IEntityView? view = null)
        where T : class, IEntity
        => GetRepository<T>().Query(view);

    public T MakeEntity<T>(Guid? id = null)
        where T : class, IEntity
        => GetRepository<T>().MakeEntity(id);

    public Task SubLoad<T>(
        T entity,
        IEntityView view,
        CancellationToken cancellationToken = default)
        where T : class, IEntity
        => entityViewLoader.LoadAsync(entity, view, cancellationToken);

    public void Update(IEntity model)
        => UpdateDetached(model);

    public void Update(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        foreach (var model in models)
            UpdateDetached(model);
    }

    public void Create(IEntity model)
        => dbContext.Add(model);

    public void Create(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        dbContext.AddRange(models.Cast<object>().ToArray());
    }

    public void Delete(IEntity model)
        => dbContext.Remove(model);

    public void Delete(IEnumerable<IEntity> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        dbContext.RemoveRange(models.Cast<object>().ToArray());
    }

    public void Delete<T>(IEnumerable<T> models)
        where T : IEntity
    {
        ArgumentNullException.ThrowIfNull(models);
        dbContext.RemoveRange(models.Cast<object>().ToArray());
    }

    public void Delete<T>(Guid id)
        where T : class, IEntity
        => GetRepository<T>().DeleteById(id);

    public void Delete<T>(IReadOnlyCollection<Guid> ids)
        where T : class, IEntity
        => GetRepository<T>().DeleteByIds(ids);

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        var initialChanges = GetChanged();
        await entityChangeProcessor.ProcessAsync(initialChanges);
        var finalChanges = GetChanged();
        await CreateAuditEvents(finalChanges);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    protected virtual List<EntityEntry> GetChanged()
        => dbContext
            .ChangeTracker
            .Entries()
            .Where(entry => entry.State is
                EntityState.Modified or
                EntityState.Added or
                EntityState.Deleted)
            .ToList();

    protected virtual async Task CreateAuditEvents(
        IReadOnlyCollection<EntityEntry> changes)
    {
        var auditWriter = auditWriterLazy.Value;

        foreach (var entry in changes)
        {
            await auditWriter.AddEvent(new AuditRequest
            {
                Entity = (IEntity)entry.Entity,
                EventType = entry.State switch
                {
                    EntityState.Deleted => AuditEventType.Deleted,
                    EntityState.Modified => AuditEventType.Updated,
                    EntityState.Added => AuditEventType.Created,
                    _ => throw new InvalidOperationException()
                }
            });
        }
    }

    protected virtual IEntityRepository<T> GetRepository<T>()
        where T : class, IEntity
        => entityRepositoryResolver.Resolve<T>(entityRepositoryKey);

    private void UpdateDetached(IEntity model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entry = dbContext.Entry(model);
        if (entry.State == EntityState.Detached)
        {
            // Update сохраняет семантику обхода графа для detached-моделей.
            dbContext.Update(model);
        }
    }
}

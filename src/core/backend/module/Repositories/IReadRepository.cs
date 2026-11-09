namespace Tmobis.Core.Repositories;

using System.Linq.Expressions;
using Entities;
using Models;

public interface IReadRepository
{
    public Task<T?> GetById<T>(
        Guid id,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<List<T>> GetByIds<T>(
        IReadOnlyCollection<Guid> ids,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<List<T>> Find<T>(
        Expression<Func<T, bool>>? predicate = null,
        IEntityView? view = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<T?> FirstOrDefault<T>(
        Expression<Func<T, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<List<T>> All<T>(
        IEntityView? view = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<bool> Any<T>(
        Expression<Func<T, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public Task<int> Count<T>(
        Expression<Func<T, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;

    public IQueryable<T> Query<T>(IEntityView? view = null)
        where T : class, IEntity;

    public Task SubLoad<T>(
        T entity,
        IEntityView view,
        CancellationToken cancellationToken = default)
        where T : class, IEntity;
}

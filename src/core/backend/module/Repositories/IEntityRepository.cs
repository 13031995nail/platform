namespace Tmobis.Core.Repositories;

using System.Linq.Expressions;
using Entities;
using Models;

public interface IEntityRepository<TI>
    where TI : class, IEntity
{
    /// <summary>
    /// Получение сущности по ID.
    /// </summary>
    /// <param name="id">ID сущности.</param>
    /// <param name="view">Представление для выгрузки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Сущность.</returns>
    public Task<TI?> GetById(
        Guid id,
        IEntityView? view = null,
        CancellationToken cancellationToken = default);

    public Task<List<TI>> GetByIds(
        IReadOnlyCollection<Guid> ids,
        IEntityView? view = null,
        CancellationToken cancellationToken = default);

    public Task<List<TI>> Find(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);

    public Task<TI?> FirstOrDefault(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default);

    public Task<List<TI>> All(
        IEntityView? view = null,
        Func<IQueryable<TI>, IOrderedQueryable<TI>>? orderBy = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default);

    public Task<bool> Any(
        Expression<Func<TI, bool>> predicate,
        IEntityView? view = null,
        CancellationToken cancellationToken = default);

    public Task<int> Count(
        Expression<Func<TI, bool>>? predicate = null,
        IEntityView? view = null,
        CancellationToken cancellationToken = default);

    public IQueryable<TI> Query(IEntityView? view = null);

    public TI MakeEntity(Guid? id = null);

    public void DeleteById(Guid id);

    public void DeleteByIds(IReadOnlyCollection<Guid> ids);
}

public interface IEntityRepository<TI, T>
    : IEntityRepository<TI>
    where TI : class, IEntity
    where T : TI
{
}

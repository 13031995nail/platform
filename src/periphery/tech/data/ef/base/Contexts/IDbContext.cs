namespace Tmobis.Data.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tmobis.Core.Models;

public interface IDbContext
{
    public ChangeTracker ChangeTracker { get; }

    public EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
    where TEntity : class;
    
    public int SaveChanges();
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// Получить DbSet конкретной сущности.
    /// На данный момент не работает с интерфейсными сущностями.
    /// </summary>
    DbSet<TModel> GetTable<TModel>()
    where TModel: class, IEntity;

    EntityEntry<TModel> Update<TModel>(TModel entity)
    where TModel: class;
    
    EntityEntry<TModel> Remove<TModel>(TModel entity)
    where TModel: class;

    EntityEntry<TModel> Add<TModel>(TModel entity)
    where TModel: class;

    EntityEntry<TModel> Attach<TModel>(TModel entity)
    where TModel: class;

    void AddRange(params object[] entities);
    
    void AttachRange(params object[] entities);
    
    void UpdateRange(params object[] entities);
    
    void RemoveRange(params object[] entities);
}
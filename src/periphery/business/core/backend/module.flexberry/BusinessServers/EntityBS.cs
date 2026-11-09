namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using ICSSoft.STORMNET;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Models;

public abstract class EntityBS<TEntity>
: ICSSoft.STORMNET.Business.BusinessServer
where TEntity : class, IEntity
{
    protected virtual List<IEntity> OnEntityChanged(TEntity entity)
    {
        var task = OnEntityChangedAsync(entity);
        task.Wait();
        return task.Result;
    }

    protected virtual async Task<List<IEntity>> OnEntityChangedAsync(
      TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var dataObject = entity as DataObject
            ?? throw new InvalidOperationException("Ошибка при касте сущности");

        var status = dataObject.GetStatus();
        return status switch
        {
            ObjectStatus.UnAltered => [],
            ObjectStatus.Deleted => await OnEntityDeleted(entity),
            ObjectStatus.Created => await OnEntityCreated(entity),
            ObjectStatus.Altered => await OnEntityUpdated(entity),
            _ => []
        };
    }

    protected virtual async Task<List<IEntity>> OnEntityCreated(TEntity entity)
    => [];

    protected virtual async Task<List<IEntity>> OnEntityUpdated(TEntity entity)
    => [];

    protected virtual async Task<List<IEntity>> OnEntityDeleted(TEntity entity)
    => [];
}

public abstract class MessageBasedEntityBS<TEntity>(
    IMessageDispatcher messageDispatcher)
: EntityBS<TEntity>
where TEntity : class, IEntity
{
    protected override Task<List<IEntity>> OnEntityCreated(TEntity entity)
    => PublishChange(
        new EntityCreated<TEntity> { Entity = entity });

    protected override Task<List<IEntity>> OnEntityUpdated(TEntity entity)
    => PublishChange(
        new EntityUpdated<TEntity> { Entity = entity });

    protected override Task<List<IEntity>> OnEntityDeleted(TEntity entity)
    => PublishChange(
        new EntityDeleted<TEntity> { Entity = entity });

    protected virtual async Task<List<IEntity>> PublishChange(
        IEntityUpdated<TEntity> action)
    => [.. (await messageDispatcher
        .PublishChainWithResult(action)
        .ConfigureAwait(true))
        .Cast<IEntityUpdatedResult>()
        .Select(result => result.EntitiesToUpdate)
        .SelectMany(result => result)
        .Distinct()
        .Where(entity => entity != null)];
}

namespace Tmobis.Core.Entities.Consumers;

using Repositories;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Models;

public abstract class OnEntityUpdated<TRequest, TEntity>(
    IRepository? repository)
    : IMessageConsumer<TRequest, IEntityUpdatedResult>
    where TRequest : IEntityUpdated<TEntity>
    where TEntity : class, IEntity
{
    protected virtual IEntityView? PreloadView => null; 
    
    public virtual async Task<IEntityUpdatedResult> ConsumeWithResult(
        TRequest message)
    {
        var entity = message.Entity;
        await PrepareEntity(entity);
        var toUpdate = await ChangeEntity(entity);
        await FinishEntity(entity);
        // TODO: Учесть свойство Continue
        return new EntityUpdatedResult { EntitiesToUpdate = toUpdate };
    }

    protected virtual async ValueTask PrepareEntity(TEntity entity)
    {
        if (PreloadView != null && repository != null)
        {
            await repository.SubLoad(entity, PreloadView);
        }
    }

    protected abstract ValueTask<List<IEntity>> ChangeEntity(TEntity entity);

    protected virtual async ValueTask FinishEntity(TEntity entity)
    {
    }
}

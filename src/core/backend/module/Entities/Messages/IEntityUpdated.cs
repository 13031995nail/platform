namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public interface IEntityUpdated<TEntity>
: IEntityMessage<TEntity>, IMessage<IEntityUpdatedResult>
where TEntity : IEntity
{
}
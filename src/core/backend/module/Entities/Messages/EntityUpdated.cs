namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public record EntityUpdated<TEntity>
: EntityMessage<TEntity>, IEntityUpdated<TEntity>
where TEntity : IEntity
{
}
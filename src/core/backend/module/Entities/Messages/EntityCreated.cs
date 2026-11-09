namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public record EntityCreated<TEntity>
: EntityUpdated<TEntity>, IEntityCreated<TEntity>
where TEntity : IEntity
{
}
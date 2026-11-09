namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public record EntityDeleted<TEntity>
: EntityUpdated<TEntity>, IEntityDeleted<TEntity>
where TEntity : IEntity
{
}
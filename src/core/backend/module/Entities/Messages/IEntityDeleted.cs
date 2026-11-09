namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public interface IEntityDeleted<TEntity>
: IEntityUpdated<TEntity>
where TEntity : IEntity
{
}
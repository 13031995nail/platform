namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public interface IEntityCreated<TEntity>
: IEntityUpdated<TEntity>
where TEntity : IEntity
{
}
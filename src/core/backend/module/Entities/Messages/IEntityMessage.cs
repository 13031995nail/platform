namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public interface IEntityMessage<TEntity> : IMessage
where TEntity : IEntity
{
    TEntity Entity { get; init; }
}
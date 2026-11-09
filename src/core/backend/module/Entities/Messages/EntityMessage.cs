namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public record EntityMessage<TEntity> : Message
where TEntity : IEntity
{
    public required TEntity Entity { get; init; }
}
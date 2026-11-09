namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public interface IEntityUpdatedResult
: IChainMessageResult
{
    List<IEntity> EntitiesToUpdate { get; init; }
}
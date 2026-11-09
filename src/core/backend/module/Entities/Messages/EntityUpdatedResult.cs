namespace Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public record EntityUpdatedResult
: IEntityUpdatedResult
{
    public List<IEntity> EntitiesToUpdate { get; init; } = [];

    public bool Continue { get; init; } = true;
}
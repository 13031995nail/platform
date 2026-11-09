namespace Tmobis.Core.Entities.Filters;

public interface IEntityQueryFilter
{
    List<EntityQueryFilterOperation> Operations { get; init; }
}

public record EntityQueryFilter(
    List<EntityQueryFilterOperation> Operations)
: IEntityQueryFilter;
namespace Tmobis.Core.Entities.Messages;

using Tmobis.Core.Entities.Filters;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Models;

public abstract record GetEntityQuery<TEntity>
    : DomainQuery<EntityQueryResult<TEntity>>
    where TEntity : class, IEntity;

public abstract record GetEntityByIdQuery<TEntity>
    : GetEntityQuery<TEntity>
    where TEntity : class, IEntity
{
    public Guid Id { get; init; }
}

public abstract record GetEntityListQuery<TEntity>
    : DomainQuery<EntityListQueryResult<TEntity>>
    where TEntity : class, IEntity;

public abstract record GetPagedEntityListQuery<TEntity>
    : GetEntityListQuery<TEntity>
    where TEntity : class, IEntity
{
    public int? Page { get; init; } = 1;

    public int? PerPage { get; init; }

    public string? Sorting { get; init; }

    public bool IncludeTotalCount { get; init; }
}

public abstract record GetFilteredEntityListQuery<TEntity>
    : GetPagedEntityListQuery<TEntity>
    where TEntity : class, IEntity
{
    public EntityQueryFilter? Filter { get; init; }

    public string? Search { get; init; }
}

public record EntityQueryResult<TEntity>(
    TEntity? Entity)
: IMessageResult
    where TEntity : class, IEntity;

public record EntityListQueryResult<TEntity>(
    List<TEntity> Data,
    int? Page,
    int? PerPage,
    int? TotalCount = null)
: IMessageResult
    where TEntity : class, IEntity;

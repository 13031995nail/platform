namespace Tmobis.Data.EF.Repositories;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Core.Models;
using Tmobis.Core.Reflection;

/// <summary>
/// Ограничитель получения данных для EF запросов.
/// </summary>
public interface IEFEntityAccessPolicy<TI, T>
    where TI : class, IEntity
    where T : BusinessEntity, TI, new()
{
    IQueryable<T> Apply(IQueryable<T> query);

    ValueTask EnsureCurrentTownAsync(
        CancellationToken cancellationToken = default);

    ValueTask<bool> IsAllowedLocalAsync(
        T entity,
        CancellationToken cancellationToken = default);

    ValueTask<HashSet<Guid>> FilterAllowedLocalIdsAsync(
        IReadOnlyCollection<T> entities,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Ограничитель по филиалу.
/// </summary>
public sealed class EFTownLimitPolicy<TI, T>(
    IDbContext databaseContext,
    ITownLimitResolver townLimitResolver,
    ICurrentTownProvider currentTownProvider)
    : IEFEntityAccessPolicy<TI, T>
    where TI : class, IEntity
    where T : BusinessEntity, TI, new()
{
    private readonly DbSet<T> dbSet =
        databaseContext.GetTable<T>();

    // Статический кэш в рамках generic для быстрого построения ограничений.
    // MedatadaLock нужен для первой инициализации кэша.
    private static readonly object metadataLock = new();
    private static TownLimitCacheEntry? cachedEntry;

    public IQueryable<T> Apply(IQueryable<T> query)
    {
        var entry = GetTownLimitEntry();
        if (entry.TownLimit == null)
            return query;

        var townId = GetCachedTownId();
        return Apply(query, entry, townId);
    }

    public async ValueTask EnsureCurrentTownAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entry = GetTownLimitEntry();
        if (entry.TownLimit == null)
            return;

        if (currentTownProvider.TryGetCachedTownId(out _))
            return;

        await currentTownProvider.GetCurrentTownId();
    }

    public async ValueTask<bool> IsAllowedLocalAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        var entry = GetTownLimitEntry();
        if (entry.TownLimit == null)
            return true;

        var townId = GetCachedTownId()
            ?? throw new InvalidOperationException(
                "У пользователя не определен филиал");

        if (entry.LocalTownIdAccessor != null)
            return entry.LocalTownIdAccessor(entity) == townId;

        var predicate = entry.PredicateFactory(townId);

        return await dbSet
            .Where(predicate)
            .AnyAsync(
                candidate => candidate.Id == entity.Id,
                cancellationToken);
    }

    public async ValueTask<HashSet<Guid>> FilterAllowedLocalIdsAsync(
        IReadOnlyCollection<T> entities,
        CancellationToken cancellationToken = default)
    {
        if (entities.Count == 0)
            return [];

        var entry = GetTownLimitEntry();

        if (entry.TownLimit == null)
        {
            return entities
                .Select(entity => entity.Id)
                .ToHashSet();
        }

        var townId = GetCachedTownId()
            ?? throw new InvalidOperationException(
                "У пользователя не определен филиал");

        if (entry.LocalTownIdAccessor != null)
        {
            return entities
                .Where(entity =>
                    entry.LocalTownIdAccessor(entity) == townId)
                .Select(entity => entity.Id)
                .ToHashSet();
        }

        var ids = entities
            .Select(entity => entity.Id)
            .Distinct()
            .ToList();

        var predicate = entry.PredicateFactory(townId);

        return (await dbSet
                .Where(predicate)
                .Where(entity => ids.Contains(entity.Id))
                .Select(entity => entity.Id)
                .ToListAsync(cancellationToken))
            .ToHashSet();
    }

    private IQueryable<T> Apply(
        IQueryable<T> query,
        TownLimitCacheEntry entry,
        Guid? townId)
    {
        if (!townId.HasValue)
        {
            throw new InvalidOperationException(
                "У пользователя не определен филиал");
        }

        return query.Where(
            entry.PredicateFactory(townId.Value));
    }

    private Guid? GetCachedTownId()
    {
        if (!currentTownProvider.TryGetCachedTownId(out var townId))
        {
            throw new InvalidOperationException(
                "Филиал текущего пользователя ещё не получен. " +
                "Синхронное построение запроса требует предварительно загруженный филиал.");
        }

        return townId;
    }

    private TownLimitCacheEntry GetTownLimitEntry()
    {
        var entry = cachedEntry;
        if (entry != null)
            return entry;

        lock (metadataLock)
        {
            entry = cachedEntry;
            if (entry != null)
                return entry;

            var townLimit =
                townLimitResolver.GetTownLimit(typeof(TI));

            entry = townLimit == null
                ? TownLimitCacheEntry.NoLimit
                : BuildTownLimitCacheEntry(townLimit);

            cachedEntry = entry;
            return entry;
        }
    }

    private static TownLimitCacheEntry BuildTownLimitCacheEntry(
        TownLimitDefinition townLimit)
    {
        var entityParam =
            Expression.Parameter(typeof(T), "entity");

        var path = townLimit.TownPropertyPath.Split(
            '.',
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries);

        if (path.Length == 0)
        {
            throw new InvalidOperationException(
                $"Не указан путь филиального ограничения для типа \"{typeof(T).FullName}\".");
        }

        Expression townIdProperty;
        Func<T, Guid?>? localTownIdAccessor = null;

        if (path.Length == 1 &&
            PropertyExtractor.TryExtractGuidPropertyAccess(
                entityParam,
                $"{path[0]}Id",
                out var directTownIdProperty))
        {
            townIdProperty = directTownIdProperty;
            localTownIdAccessor =
                BuildLocalTownIdAccessor(
                    entityParam,
                    townIdProperty);
        }
        else
        {
            var townProperty = PropertyExtractor
                .ExtractPropertyPathAccess(
                    entityParam,
                    townLimit.TownPropertyPath)
                .AccessExpression;

            townIdProperty =
                BuildTownIdAccess(townProperty);
        }

        Func<Guid, Expression<Func<T, bool>>> predicateFactory =
            townId =>
            {
                var concreteTownId =
                    townIdProperty.Type == typeof(Guid?)
                        ? Expression.Constant(
                            (Guid?)townId,
                            typeof(Guid?))
                        : Expression.Constant(
                            townId,
                            typeof(Guid));

                var equal =
                    Expression.Equal(
                        townIdProperty,
                        concreteTownId);

                return Expression.Lambda<Func<T, bool>>(
                    equal,
                    entityParam);
            };

        return new TownLimitCacheEntry
        {
            TownLimit = townLimit,
            PredicateFactory = predicateFactory,
            LocalTownIdAccessor = localTownIdAccessor
        };
    }

    private static Func<T, Guid?> BuildLocalTownIdAccessor(
        ParameterExpression entityParam,
        Expression townIdProperty)
    {
        Expression body = townIdProperty.Type switch
        {
            var type when type == typeof(Guid)
                => Expression.Convert(
                    townIdProperty,
                    typeof(Guid?)),

            var type when type == typeof(Guid?)
                => townIdProperty,

            _ => throw new InvalidOperationException(
                $"Town id property '{townIdProperty}' must be Guid or Guid?.")
        };

        return Expression
            .Lambda<Func<T, Guid?>>(body, entityParam)
            .Compile();
    }

    private static Expression BuildTownIdAccess(
        Expression townProperty)
    {
        if (PropertyExtractor.TryExtractGuidIdAccess(
                townProperty,
                out var idProperty))
        {
            return idProperty;
        }

        throw new InvalidOperationException(
            $"Свойство \"{townProperty.Type.FullName}\" из филиального ограничения типа \"{typeof(T).FullName}\" не содержит Guid-идентификатор.");
    }

    private sealed class TownLimitCacheEntry
    {
        public static readonly TownLimitCacheEntry NoLimit = new()
        {
            TownLimit = null,
            PredicateFactory = _ =>
                throw new InvalidOperationException(
                    "Town limit not found")
        };

        public TownLimitDefinition? TownLimit { get; init; }

        public Func<Guid, Expression<Func<T, bool>>>
            PredicateFactory { get; init; } = null!;

        public Func<T, Guid?>?
            LocalTownIdAccessor { get; init; }
    }
}

/// <summary>
/// Пустое ограничение EF запросов.
/// </summary>
internal sealed class EFNoAccessPolicy<TI, T>
    : IEFEntityAccessPolicy<TI, T>
    where TI : class, IEntity
    where T : BusinessEntity, TI, new()
{
    public static readonly EFNoAccessPolicy<TI, T> Instance = new();

    private EFNoAccessPolicy()
    {
    }

    public IQueryable<T> Apply(IQueryable<T> query)
        => query;

    public ValueTask EnsureCurrentTownAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> IsAllowedLocalAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(true);
    }

    public ValueTask<HashSet<Guid>> FilterAllowedLocalIdsAsync(
        IReadOnlyCollection<T> entities,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(
            entities
                .Select(entity => entity.Id)
                .ToHashSet());
    }
}

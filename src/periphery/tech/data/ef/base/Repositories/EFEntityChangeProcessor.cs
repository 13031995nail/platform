namespace Tmobis.Data.EF.Repositories;

using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tmobis.Core.Entities;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Models;

/// <summary>
/// Выполнение бизнес логики для измененных сущностей.
/// </summary>
public interface IEFEntityChangeProcessor
{
    Task ProcessAsync(IReadOnlyCollection<EntityEntry> changedEntries);
}

public sealed class EFEntityChangeProcessor(
    IDbContext dbContext,
    IMessageDispatcher dispatcher)
    : IEFEntityChangeProcessor
{
    /// <summary>
    /// Кэш сложных вычислений (reflection, expression).
    /// </summary>
    private static readonly ConcurrentDictionary<
        Type, EntityDispatchDescriptor[]> entityDispatchCache = new();

    public async Task ProcessAsync(
        IReadOnlyCollection<EntityEntry> changedEntries)
    {
        var processedEntities =
            new HashSet<object>(ReferenceEqualityComparer.Instance);

        var toProcess = new Queue<EntityEntry>(changedEntries);

        while (toProcess.Count > 0)
        {
            var entry = toProcess.Dequeue();

            if (!processedEntities.Add(entry.Entity))
                continue;

            if (entry.State is not (
                EntityState.Modified or
                EntityState.Added or
                EntityState.Deleted))
            {
                continue;
            }

            var descriptors = entityDispatchCache.GetOrAdd(
                entry.Entity.GetType(),
                BuildDispatchDescriptors);

            foreach (var descriptor in descriptors)
            {
                var message =
                    descriptor.CreateMessage(entry.Entity, entry.State);

                var newlyChanged = (await dispatcher
                        .PublishChainWithResult((IMessage)message)
                        .ConfigureAwait(true))
                    .Cast<IEntityUpdatedResult>()
                    .Select(result => result.EntitiesToUpdate)
                    .SelectMany(entities => entities)
                    .Distinct(ReferenceEqualityComparer.Instance)
                    .Select(entity => dbContext.Entry(entity))
                    .Where(newEntry =>
                        !processedEntities.Contains(newEntry.Entity))
                    .ToList();

                foreach (var newEntry in newlyChanged)
                    toProcess.Enqueue(newEntry);
            }
        }
    }

    private static EntityDispatchDescriptor[] BuildDispatchDescriptors(
        Type entityType)
        => entityType
            .GetInterfaces()
            .Where(iface => iface.IsAssignableTo(typeof(IEntity)))
            .Select(ifaceType => new EntityDispatchDescriptor(ifaceType))
            .ToArray();
}

/// <summary>
/// Синглтон маппинг IEntity -> сообщение об изменение сущности с конкретным материализованным типом сущности 
/// </summary>
internal readonly struct EntityDispatchDescriptor
{
    private readonly Func<object, object> createCreatedMessage;
    private readonly Func<object, object> createUpdatedMessage;
    private readonly Func<object, object> createDeletedMessage;

    public EntityDispatchDescriptor(Type ifaceType)
    {
        createCreatedMessage =
            BuildCreateMessageDelegate(ifaceType, typeof(EntityCreated<>));

        createUpdatedMessage =
            BuildCreateMessageDelegate(ifaceType, typeof(EntityUpdated<>));

        createDeletedMessage =
            BuildCreateMessageDelegate(ifaceType, typeof(EntityDeleted<>));
    }

    public object CreateMessage(object entity, EntityState state)
        => state switch
        {
            EntityState.Added => createCreatedMessage(entity),
            EntityState.Modified => createUpdatedMessage(entity),
            EntityState.Deleted => createDeletedMessage(entity),
            _ => throw new ArgumentOutOfRangeException(
                nameof(state), state, null)
        };

    private static Func<object, object> BuildCreateMessageDelegate(
        Type ifaceType,
        Type openMessageType)
    {
        var entityParam =
            Expression.Parameter(typeof(object), "entity");

        var castEntity =
            Expression.Convert(entityParam, ifaceType);

        var closedType =
            openMessageType.MakeGenericType(ifaceType);

        var entityProperty = closedType.GetProperty("Entity")
            ?? throw new InvalidOperationException(
                $"Message type '{closedType.FullName}' does not contain an Entity property.");

        var constructor = closedType.GetConstructor(Type.EmptyTypes)
            ?? throw new InvalidOperationException(
                $"Message type '{closedType.FullName}' does not contain a parameterless constructor.");

        var memberInit = Expression.MemberInit(
            Expression.New(constructor),
            Expression.Bind(entityProperty, castEntity));

        return Expression
            .Lambda<Func<object, object>>(
                Expression.Convert(memberInit, typeof(object)),
                entityParam)
            .Compile();
    }
}

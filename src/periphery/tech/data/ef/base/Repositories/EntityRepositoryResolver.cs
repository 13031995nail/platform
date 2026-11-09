namespace Tmobis.Data.EF.Repositories;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

/// <summary>
/// Резолвер IEntity -> IEntityRepository&lt;TI,T&gt;.
/// </summary>
public interface IEntityRepositoryResolver
{
    IEntityRepository<T> Resolve<T>(string? repositoryKey = null)
        where T : class, IEntity;
}

/// <summary>
/// Резолвер IEntity -> IEntityRepository&lt;TI,T&gt;.
/// Имеет кэш IEntityRepository в рамках scope.
/// </summary>
public sealed class EntityRepositoryResolver(
    IServiceProvider provider,
    IEntityMaterializer materializer)
    : IEntityRepositoryResolver
{
    private readonly Dictionary<(Type InterfaceType, string? RepositoryKey), object>
        repositories = [];

    private static readonly ConcurrentDictionary<Type, Type>
        concreteTypeCache = new();

    private static readonly ConcurrentDictionary<
        (Type InterfaceType, Type ConcreteType), Type>
        closedGenericCache = new();

    public IEntityRepository<T> Resolve<T>(string? repositoryKey = null)
        where T : class, IEntity
    {
        var interfaceType = typeof(T);
        var cacheKey = (interfaceType, repositoryKey);

        if (!repositories.TryGetValue(cacheKey, out var repository))
        {
            var concreteType = concreteTypeCache.GetOrAdd(
                interfaceType,
                type => materializer.ResolveBusinessEntityType(type));

            var repositoryType = closedGenericCache.GetOrAdd(
                (interfaceType, concreteType),
                pair => typeof(IEntityRepository<,>)
                    .MakeGenericType(pair.InterfaceType, pair.ConcreteType));

            repository = repositoryKey == null
                ? provider.GetService(repositoryType)
                : provider.GetKeyedService(repositoryType, repositoryKey);

            if (repository == null)
            {
                throw new InvalidOperationException(
                    $"Не удалось получить репозиторий '{repositoryType.FullName}'" +
                    (repositoryKey == null
                        ? "."
                        : $" с ключом '{repositoryKey}'."));
            }

            repositories.Add(cacheKey, repository);
        }

        return repository as IEntityRepository<T>
            ?? throw new InvalidOperationException(
                $"Репозиторий для '{interfaceType.FullName}' имеет несовместимый тип.");
    }
}

namespace Tmobis.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Models;

public static class EntityModuleExtensions
{
    public static void MapBusinessEntity<TI, TC>(
        this IServiceCollection services)
    where TI : class, IEntity
    where TC : class, TI, IBusinessEntity
    {
        services.AddKeyedTransient<TI, TC>("Business");
    }

    public static void MapStoredEntity<TI, TC>(
        this IServiceCollection services)
    where TI : class, IEntity
    where TC : class, TI, IStoredEntity
    {
        services.AddKeyedTransient<TI, TC>("Stored");
    }
}

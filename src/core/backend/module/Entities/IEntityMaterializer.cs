namespace Tmobis.Core.Entities;

using System.Collections.ObjectModel;
using Tmobis.Core.Models;

public interface IEntityMaterializer
{
    ReadOnlyDictionary<Type, Type> StoredMaterializations { get; }

    ReadOnlyDictionary<Type, Type> BusinessMaterializations { get; }
    
    IReadOnlyDictionary<string, EntityView> EntityViews { get; }

    Type ResolveBusinessEntityType(Type entityType);

    Type ResolveBusinessEntityType<T>()
        where T : class, IEntity;

    Type ResolveStoredEntityType(Type entityType);

    Type ResolveStoredEntityType<T>()
        where T : class, IEntity;

    IEntityView? GetViewName<T>(string? viewName);
}

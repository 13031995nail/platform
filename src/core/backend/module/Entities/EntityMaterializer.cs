namespace Tmobis.Core.Entities;

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tmobis.Core.Models;

public class EntityMaterializer
: IEntityMaterializer
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEntityRegister entityRegister;
    private readonly Dictionary<Type, Type> businessMaterializations = [];
    private readonly Dictionary<Type, Type> storedMaterializations = [];
    private readonly Dictionary<string, EntityView> entityViews = [];

    public ReadOnlyDictionary<Type, Type> StoredMaterializations
    => storedMaterializations.AsReadOnly();

    public ReadOnlyDictionary<Type, Type> BusinessMaterializations
    => businessMaterializations.AsReadOnly();

    public IReadOnlyDictionary<string, EntityView> EntityViews
        => entityViews.AsReadOnly();

    public EntityMaterializer(
      IServiceProvider serviceProvider,
      IEntityRegister entityRegister)
    {
        this.serviceProvider = serviceProvider;
        this.entityRegister = entityRegister;
        LoadMaterializations();
        entityViews = LoadEntityViews();
    }

    public virtual Type ResolveBusinessEntityType<T>()
    where T : class, IEntity
    => ResolveBusinessEntityType(typeof(T));

    public virtual Type ResolveBusinessEntityType(Type entityType)
    => ResolveEntityType(
        businessMaterializations, typeof(IBusinessEntity), entityType);

    public virtual Type ResolveStoredEntityType<T>()
    where T : class, IEntity
    => ResolveStoredEntityType(typeof(T));

    public virtual Type ResolveStoredEntityType(Type entityType)
    => ResolveEntityType(
        storedMaterializations, typeof(IStoredEntity), entityType);

    public virtual IEntityView? GetViewName<T>(string? viewName)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            return null;
        }

        var entityName = ResolveBusinessEntityType(typeof(T)).Name;
        return EntityViews.TryGetValue($"{entityName}.{viewName}", out var view)
            ? view
            : null;
    }

    protected virtual Type ResolveEntityType(
        Dictionary<Type, Type> materializations,
        Type qualifierType,
        Type modelType)
    {
        if (modelType.IsClass)
        {
            if (modelType.IsAssignableTo(qualifierType))
            {
                return modelType;
            }
            else
            {
                modelType = modelType
                .GetInterfaces()
                .FirstOrDefault(iface =>
                    iface.IsAssignableTo(typeof(IRegisteredEntity)))
                ?? throw new InvalidOperationException(
                    "Сущность не является регистрируемой");
            }
        }

        if (materializations.TryGetValue(modelType, out var materialized))
        {
            return materialized;
        }

        throw new InvalidOperationException(
            "Не найдена материализация сущности");
    }

    protected virtual void LoadMaterializations()
    {
        var storedIface = typeof(IStoredEntity);
        var businessIface = typeof(IBusinessEntity);

        foreach (var iface in entityRegister.EntityInterfaces)
        {
            var business = FindMaterialization(iface, businessIface);
            if (business != null)
            {
                AddMaterialization(
                    businessMaterializations,
                    iface,
                    business);
            }
            
            var stored = FindMaterialization(iface, storedIface);
            if (stored != null)
            {
                AddMaterialization(
                    storedMaterializations,
                    iface,
                    stored);
            }
        }
    }

    protected virtual void AddMaterialization(
        Dictionary<Type, Type> materializations,
        Type entityInterface,
        Type materialization)
    {
        materializations.Add(entityInterface, materialization);

        var extendedInterfaces = materialization
            .GetInterfaces()
            .Where(iface => iface != entityInterface
                && iface.IsAssignableTo(entityInterface));
        foreach (var extendedInterface in extendedInterfaces)
        {
            materializations.TryAdd(extendedInterface, materialization);
        }
    }

    protected virtual Dictionary<string, EntityView> LoadEntityViews()
    {
        var allViews = new Dictionary<string, EntityView>();
        foreach (var iface in entityRegister.EntityInterfaces)
        {
            var viewClass = iface.GetNestedType("Views");
            var viewFields
                = viewClass?.GetFields(BindingFlags.Static | BindingFlags.Public) ?? [];

            var materialized = ResolveBusinessEntityType(iface);
            var entityName = materialized.Name;

            foreach (var viewField in viewFields)
            {
                if (viewField.GetValue(null) is not EntityView view) continue;
                allViews.Add($"{entityName}.{view.ViewName}", view);
            }
        }

        return allViews;
    }

    protected virtual Type? FindMaterialization(
        Type iface, Type qualifierType)
    => GetEntityTypeFromKeyed(iface, qualifierType)
    ?? GetEntityTypeFromRegistry(iface, qualifierType)
    ?? GetEntityTypeFromRegistry(iface);

    protected virtual Type? GetEntityTypeFromKeyed(
        Type modelType, Type qualifierType)
    => serviceProvider
    .GetKeyedService(modelType, qualifierType.Name)
    ?.GetType();

    protected virtual Type? GetEntityTypeFromRegistry(
      Type modelType, Type? qualifierType = null)
    => entityRegister.EntityTypes
        .FirstOrDefault(type
          => type.IsAssignableTo(modelType)
            && (qualifierType == null
              || type.IsAssignableTo(qualifierType)));
}

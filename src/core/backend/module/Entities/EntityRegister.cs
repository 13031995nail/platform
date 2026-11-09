namespace Tmobis.Core.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tmobis.Core.Modules;
using Tmobis.Core.Models;
using System.Linq;

public class EntityRegister : IEntityRegister
{
    private readonly IModuleRegister modules;
    private readonly HashSet<Type> entityTypes;
    private readonly HashSet<Type> entityInterfaces;
    private readonly HashSet<Assembly> entityAssemblies;

    protected virtual List<Type> BaseEntityTypes => [typeof(IEntity)];

    public IReadOnlyCollection<Type> EntityTypes
    => entityTypes.AsReadOnly();

    public IReadOnlyCollection<Type> EntityInterfaces
    => entityInterfaces.AsReadOnly();

    public IReadOnlyCollection<Assembly> EntityAssemblies
    => entityAssemblies.AsReadOnly();

    public EntityRegister(IModuleRegister modules)
    {
        this.modules = modules;
        entityAssemblies = LoadEntityAssemblies();
        entityTypes = LoadEntityTypes();
        entityInterfaces = LoadEntityInterfaces();
    }

    public virtual Type? GetTypeByName(string name)
    => EntityTypes.FirstOrDefault(
      type => type.FullName == name);

    protected virtual HashSet<Assembly> LoadEntityAssemblies()
    {
        var assemblies = new HashSet<Assembly>();
        foreach (var module in modules)
        {
            if (module is IEntityModule entityModule)
            {
                assemblies.Add(entityModule.GetType().Assembly);
            }

            if (module is IEntityModuleExternal entityModuleExternal)
            {
                var additional = entityModuleExternal.GetAdditionalModelAssemblies();
                foreach (var additionalAssembly in additional)
                {
                    assemblies.Add(additionalAssembly);
                }
            }
        }

        return assemblies;
    }

    protected virtual HashSet<Type> LoadEntityTypes()
    {
        var types = new HashSet<Type>();
        foreach (var assembly in entityAssemblies)
        {
            var typesInAssembly = assembly
              .GetTypes()
              .Where(type => BaseEntityTypes
                .Any(baseType => type.IsAssignableTo(baseType)) && type.IsClass);

            foreach (var type in typesInAssembly)
            {
                types.Add(type);
            }
        }

        return types;
    }

    protected virtual HashSet<Type> LoadEntityInterfaces()
    {
        var types = new HashSet<Type>();
        foreach (var type in entityTypes)
        {
            var entityIface = type
                .GetInterfaces()
                .LastOrDefault(iface =>
                    GetDirectInterfaces(iface).Contains(typeof(IRegisteredEntity)));

            if (entityIface != null)
            {
                types.Add(entityIface);
            }
        }

        return types;
    }

    private IEnumerable<Type> GetDirectInterfaces(Type iface)
    {
        var all = iface.GetInterfaces();
        return all.Where(candidate =>
            !all.Any(other => other != candidate && other.GetInterfaces().Contains(candidate)));
    }
}

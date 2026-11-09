namespace Tmobis.Core.Entities;
using System.Collections.ObjectModel;
using System.Reflection;

public interface IEntityRegister
{
  IReadOnlyCollection<Type> EntityTypes { get; }

  IReadOnlyCollection<Type> EntityInterfaces { get; }

  IReadOnlyCollection<Assembly> EntityAssemblies { get; }

  Type? GetTypeByName(string name);
}

namespace Tmobis.Core.Modules;
using System.Reflection;

/// <summary>
/// Модуль-маркер, регистрирующий бизнес-сущности из своего проекта.
/// </summary>
public interface IEntityModule : IModule;

/// <summary>
/// Модуль-маркер, регистрирующий бизнес-сущности из сторонних библиотек.
/// </summary>
public interface IEntityModuleExternal : IModule
{
    /// <summary>
    /// Регистрация классов объектной модели (например, для OData),
    /// исключая сам модуль.
    /// </summary>
    /// <returns>Список сборок объектной модели для регистрации.</returns>
    public List<Assembly> GetAdditionalModelAssemblies();
}

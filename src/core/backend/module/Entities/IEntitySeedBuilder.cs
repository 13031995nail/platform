namespace Tmobis.Core.Entities;

/// <summary>
/// Построитель дефолтных данных сущностей.
/// </summary>
public interface IEntitySeedBuilder
{
    /// <summary>
    /// Добавление дефолтных записей сущности.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    /// <param name="entries">Дефолтные записи.</param>
    void Add<TEntity>(params TEntity[] entries) where TEntity : class;
}

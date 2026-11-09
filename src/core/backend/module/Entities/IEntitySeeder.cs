namespace Tmobis.Core.Entities;

/// <summary>
/// Наполнение модели дефолтными данными.
/// </summary>
public interface IEntitySeeder
{
    /// <summary>
    /// Регистрация дефолтных данных сущностей.
    /// </summary>
    /// <param name="builder">Построитель дефолтных данных.</param>
    void Seed(IEntitySeedBuilder builder);
}

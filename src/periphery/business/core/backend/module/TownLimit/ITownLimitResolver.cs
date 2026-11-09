namespace Tmobis.Business.Core.TownLimit;

/// <summary>
/// Определяет филиальное ограничение для типа.
/// </summary>
public interface ITownLimitResolver
{
  /// <summary>
  /// Получить описание филиального ограничения для типа.
  /// </summary>
  /// <param name="type">Тип объекта.</param>
  /// <returns>Описание ограничения или <see langword="null"/>.</returns>
  TownLimitDefinition? GetTownLimit(Type type);
}

namespace Tmobis.Business.Core.TownLimit;

/// <summary>
/// Описание филиального ограничения для типа.
/// </summary>
/// <param name="TownPropertyPath">Путь к свойству города.</param>
public sealed record TownLimitDefinition(string TownPropertyPath);

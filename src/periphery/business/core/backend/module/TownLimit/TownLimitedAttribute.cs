namespace Tmobis.Business.Core.TownLimit;
using System;

/// <summary>
/// Атрибут, помечающий, что объект нужно
/// ограничивать по городу текущего пользователя.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class TownLimitedAttribute(string path)
: Attribute
{
  /// <summary>
  /// Путь к свойству города, по которому ограничивается объект.
  /// </summary>
  public string TownPropertyPath { get; set; } = path;
}

namespace Tmobis.Business.Core.Providers.Towns;
using Tmobis.Business.Core.Models.Towns;

/// <summary>
/// Сервис получения филиалов.
/// </summary>
public interface ITownProvider
{
  /// <summary>
  /// Получить филиал по коду.
  /// </summary>
  /// <returns> Объект филиала.</returns>
  public ValueTask<ITown?> GetTownByCode(string code);
  
  /// <summary>
  /// Получить филиал по коду.
  /// </summary>
  /// <returns> Объект филиала.</returns>
  public ValueTask<ITown?> GetTownById(Guid id);
}

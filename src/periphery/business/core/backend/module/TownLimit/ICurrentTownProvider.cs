namespace Tmobis.Business.Core.TownLimit;

/// <summary>
/// Поставщик филиала текущего пользователя.
/// </summary>
public interface ICurrentTownProvider
{
  /// <summary>
  /// Получить идентификатор филиала текущего пользователя.
  /// </summary>
  /// <returns>Идентификатор филиала или <see langword="null"/>.</returns>
  ValueTask<Guid?> GetCurrentTownId();

  /// <summary>
  /// Возвращает филиал, уже полученный в текущем scope.
  /// </summary>
  bool TryGetCachedTownId(out Guid? townId);
}

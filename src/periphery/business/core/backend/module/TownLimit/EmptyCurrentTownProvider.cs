namespace Tmobis.Business.Core.TownLimit;

/// <summary>
/// Пустой поставщик филиала текущего пользователя.
/// </summary>
public class EmptyCurrentTownProvider : ICurrentTownProvider
{
  /// <inheritdoc/>
  public ValueTask<Guid?> GetCurrentTownId()
    => ValueTask.FromResult<Guid?>(null);

  /// <inheritdoc/>
  public bool TryGetCachedTownId(out Guid? townId)
  {
    townId = null;
    return true;
  }
}

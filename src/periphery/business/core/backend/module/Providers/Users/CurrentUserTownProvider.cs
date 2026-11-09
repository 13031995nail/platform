namespace Tmobis.Business.Core.Providers.Users;

using Tmobis.Business.Core.TownLimit;

/// <summary>
/// Поставщик филиала текущего пользователя.
/// </summary>
public class CurrentUserTownProvider(ICurrentUserProvider currentUserProvider)
  : ICurrentTownProvider
{
  private bool isResolved;
  private Guid? townId;

  /// <inheritdoc/>
  public bool TryGetCachedTownId(out Guid? cachedTownId)
  {
    cachedTownId = townId;
    return isResolved;
  }

  /// <inheritdoc/>
  public async ValueTask<Guid?> GetCurrentTownId()
  {
    if (isResolved)
    {
      return townId;
    }

    var currentUser = await currentUserProvider.GetCurrentUser();

    var resolvedTownId = currentUser?.TownId;
    if (resolvedTownId == null || resolvedTownId == Guid.Empty)
    {
      throw new InvalidOperationException("У пользователя не определен филиал");
    }

    townId = resolvedTownId;
    isResolved = true;
    return townId;
  }
}

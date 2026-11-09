namespace Tmobis.Frameworks.Web.Backend.Users;

using Microsoft.AspNetCore.Http;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.TownLimit;

/// <summary>
/// Использует филиал из проверенного JWT без запроса к базе.
/// Для старых токенов без claim сохраняет асинхронное получение пользователя.
/// </summary>
public sealed class WebCurrentTownProvider(
    IHttpContextAccessor httpContextAccessor,
    ICurrentUserProvider currentUserProvider)
    : ICurrentTownProvider
{
    private readonly CurrentUserTownProvider fallback = new(currentUserProvider);

    public bool TryGetCachedTownId(out Guid? townId)
    {
        var principal = httpContextAccessor.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated == true)
        {
            var claim = principal.FindFirst(TownClaimTypes.TownId);
            if (claim != null)
            {
                if (!Guid.TryParse(claim.Value, out var value) ||
                    value == Guid.Empty)
                {
                    throw new InvalidOperationException(
                        "Некорректный идентификатор филиала в токене.");
                }

                townId = value;
                return true;
            }
        }

        return fallback.TryGetCachedTownId(out townId);
    }

    public ValueTask<Guid?> GetCurrentTownId()
        => TryGetCachedTownId(out var townId)
            ? ValueTask.FromResult(townId)
            : fallback.GetCurrentTownId();
}

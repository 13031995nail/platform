namespace Tmobis.Auth.JwtAuth.Options;

using System.Security.Claims;
using Tmobis.Business.Core.TownLimit;

/// <summary>
/// Дополняет прошедшую валидацию JWT идентификатором филиала на время запроса.
/// Сам JWT не изменяется: старые токены продолжают работать до истечения срока.
/// </summary>
public static class LegacyTownClaimInitializer
{
    public static async Task<bool> EnsureTownClaimAsync(
        ClaimsPrincipal principal,
        Func<string, Task<Guid?>> resolveTownId)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(resolveTownId);

        var identity = principal.Identities
            .FirstOrDefault(item => item.IsAuthenticated);
        if (identity == null)
            return false;

        var claim = principal.FindFirst(TownClaimTypes.TownId);
        if (claim != null)
        {
            return Guid.TryParse(claim.Value, out var existingTownId)
                && existingTownId != Guid.Empty;
        }

        var login = principal.FindFirst(ClaimTypes.Name)?.Value
            ?? principal.FindFirst("unique_name")?.Value
            ?? identity.Name;
        if (string.IsNullOrWhiteSpace(login))
            return false;

        var townId = await resolveTownId(login);
        if (townId is not { } value || value == Guid.Empty)
            return false;

        identity.AddClaim(new Claim(
            TownClaimTypes.TownId,
            value.ToString("D")));
        return true;
    }
}

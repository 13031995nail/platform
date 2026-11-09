namespace Tmobis.Auth.Keycloak;

using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Options;
using Tmobis.Auth;
using Tmobis.Core.Repositories;

/// <inheritdoc/>
public class KeycloakAuthenticator(
    IUnitOfWork unitOfWork,
    ITokenAuthenticator tokenAuthenticator,
    IKeycloakTokenValidator tokenValidator,
    IKeycloakUserProvider userProviderKeycloak,
    IOptions<KeycloakAuthOptions> options)
: IKeycloakAuthenticator
{
    private readonly KeycloakAuthOptions options = options.Value;
    
    /// <inheritdoc/>
    public async Task<string> AuthenticateToken(string token)
    {
        try
        {
            var (isValid, claims) = await tokenValidator.ValidateToken(token);
            if (!isValid)
            {
                throw new InvalidOperationException("Неверный или истекший токен");
            }
            
            var roles = GetRolesFromClaims(claims);
            var user = await userProviderKeycloak.GetOrCreateUser(claims, roles);
            await unitOfWork.Commit();
            var payload = await tokenAuthenticator.Authenticate(user, roles)
                ?? throw new InvalidOperationException("Не удалось получить токен");
            return payload;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("Не удалось получить пользователя", ex);
        }
    }

    /// <summary>
    /// Получение ролей из сведений о пользователе из Keycloak.
    /// </summary>
    /// <param name="claims">Сведения о пользователе из Keycloak.</param>
    /// <returns>Список ролей.</returns>
    private List<string> GetRolesFromClaims(Dictionary<string, string> claims)
    {
        var roles = new List<string>();
        if (!claims.TryGetValue(options.RolesClaimType, out var rolesValue))
            return roles;
        
        var accountJson = JsonConvert.DeserializeObject<JObject>(rolesValue);
        roles = accountJson?["roles"] is JArray rolesJson
            ? rolesJson.Select(r => r.ToString()).ToList()
            : roles;
    
        return roles;
    }
}

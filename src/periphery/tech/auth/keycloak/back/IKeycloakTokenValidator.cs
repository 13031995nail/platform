namespace Tmobis.Auth.Keycloak;

/// <summary>
/// Сервис валидации токенов Keycloak.
/// </summary>
public interface IKeycloakTokenValidator
{
    /// <summary>
    /// Валидация токена Keycloak.
    /// </summary>
    /// <param name="accessToken">Токен из Keycloak.</param>
    /// <returns><c>true</c> - если токен валиден и пользователь существует</returns>
    Task<KeycloakTokenValidationResult> ValidateToken(string accessToken);
}

public record KeycloakTokenValidationResult(
    bool IsValid, Dictionary<string, string> Claims)
{
    public void Deconstruct(out bool isValid, out Dictionary<string, string> claims)
    {
        isValid = IsValid;
        claims = Claims;
    }
}
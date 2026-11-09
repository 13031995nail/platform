namespace Tmobis.Auth.Keycloak;

/// <summary>
/// Сервис аутентификации на основе токена Keycloak.
/// </summary>
public interface IKeycloakAuthenticator
{
    /// <summary>
    /// Аутентификация через Keycloak на основе токена Keycloak
    /// </summary>
    /// <param name="token">Токен от Keycloak.</param>
    /// <returns>Объект с токеном приложения и данными пользователя.</returns>
    Task<string> AuthenticateToken(string token);
}

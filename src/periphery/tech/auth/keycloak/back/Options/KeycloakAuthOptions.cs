namespace Tmobis.Auth.Keycloak.Options;

public class KeycloakAuthOptions
{
    public static string KeycloakAuth = "KeycloakAuth";

    /// <summary>
    /// Используется для валидации токенов от Keycloak
    /// </summary>
    public string KeycloakIssuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience для валидации токена
    /// </summary>
    public string KeycloakAudience { get; set; } = string.Empty;

    /// <summary>
    /// OpenID Connect discovery document URL (для получения публичных ключей)
    /// Если не указан, будет сформирован из Issuer
    /// </summary>
    public string? OpenIdConfigUrl { get; set; }

    /// <summary>
    /// Публичный ключ realm Keycloak (Base64 encoded)
    /// Можно использовать вместо OpenIdConfigUrl для статической конфигурации
    /// </summary>
    public string? RealmPublicKey { get; set; }

    /// <summary>
    /// Разрешить синхронизацию пользователей из Keycloak
    /// </summary>
    public bool EnableUserSync { get; set; } = true;

    /// <summary>
    /// Имя claim для логина пользователя (обычно 'preferred_username' или 'email')
    /// </summary>
    public string UsernameClaimType { get; set; } = "preferred_username";

    /// <summary>
    /// Имя claim для роли пользователя
    /// </summary>
    public string RolesClaimType { get; set; } = "realm_access";
}

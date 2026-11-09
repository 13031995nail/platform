namespace Tmobis.Auth.Keycloak;

using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис синхронизации пользователей из Keycloak.
/// </summary>
public interface IKeycloakUserProvider
{
    /// <summary>
    /// Получение или создание пользователя на основе данных из Keycloak.
    /// </summary>
    /// <param name="claims">Сведения о пользователе из Keycloak.</param>
    /// <param name="roles"></param>
    /// <returns>Объект пользователя.</returns>
    Task<IUser> GetOrCreateUser(Dictionary<string, string> claims, List<string> roles);
}

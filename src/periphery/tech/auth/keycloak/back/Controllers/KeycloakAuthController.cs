namespace Tmobis.Auth.Keycloak.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

[Route("/api/auth/keycloak")]
[ApiController]
public class KeycloakAuthController
: ControllerBase
{
    /// <summary>
    /// Аутентификация через Keycloak.
    /// Принимает access token от Keycloak и возвращает токен приложения.
    /// </summary>
    /// <param name="accessToken">Access token от Keycloak (из заголовка Authorization).</param>
    /// <param name="authenticator">...</param>
    /// <returns>Объект с токеном и данными пользователя.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(JObject), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Token(
        [FromHeader] string accessToken,
        [FromServices] IKeycloakAuthenticator authenticator)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return BadRequest("Токен не предоставлен");
        }
        
        var payload = await authenticator.AuthenticateToken(accessToken);
        return Ok(payload);
    }
}

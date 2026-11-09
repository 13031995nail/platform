namespace Tmobis.Auth.JwtAuth.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Tmobis.Auth;
using Tmobis.Business.Core.Providers.Users;

[Route("/api/auth")]
[ApiController]
public class JwtAuthController(
    IUserProvider userProvider,
    ITokenAuthenticator authenticator,
    ILogger<JwtAuthController> logger)
: ControllerBase
{
    [Authorize]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(JObject), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var username = User.FindFirstValue(ClaimTypes.Name)
                ?? throw new InvalidOperationException("Не определено имя пользователя");
            var user = await userProvider.GetUserByLogin(username)
                ?? throw new InvalidOperationException("Не найден пользователь");
            var roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value).ToList();
            var payload = await authenticator.Authenticate(user, roles)
                ?? throw new InvalidOperationException("Не удалось получить токен");
            return Ok(payload);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении токена");
            return BadRequest("Ошибка при обновлении токена");
        }
    }
}

namespace Tmobis.Auth.Password.Controllers;

using System.Threading.Tasks;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Options;
using Services;
using Tmobis.Auth;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;
using Verifiers;

[Route("/api/auth")]
[ApiController]
public class PasswordAuthController(
  IRoleProvider roleProvider,
  IUserProvider userProvider,
  IEnumerable<IUserVerifier> userVerifiers,
  ITokenAuthenticator tokenAuthenticator,
  IOptions<AuthOptions> options,
  IAuthTimeoutService authTimeout)
: ControllerBase
{
  private readonly AuthOptions options = options.Value;
  private readonly string userNotFoundMessage = "Пользователь не найден";
  private readonly string noRequestMessage = "Не удалось получить данные запроса";
  private readonly string authBlockedMessage = "Превышено доступное количество попыток входа. Пожалуйста, попробуйте снова через 5 минут";
  private readonly string wrongCredsMessage = "Неверное имя пользователя или пароль. Проверьте правильность введённых данных";

  [AllowAnonymous]
  [HttpPost("token")]
  [ProducesResponseType(typeof(JObject), 200)]
  [ProducesResponseType(typeof(string), 400)]
  public virtual async Task<IActionResult> Token(
    [FromForm] PasswordTokenRequest request)
  {
    bool useAuthTimeout = options.Timeouts.IsInUse;

    if (request == null)
    {
      return BadRequest(noRequestMessage);
    }

    var username = request.Username;
    var password = request.Password;
    if (string.IsNullOrEmpty(username)
      || string.IsNullOrEmpty(password))
    {
      return BadRequest(userNotFoundMessage);
    }

    var user = await userProvider.GetUserByLogin(username);
    if (user == null)
    {
      return BadRequest(userNotFoundMessage);
    }

    if (useAuthTimeout)
    {
      var isBlocked = authTimeout.IsBlocked(user);
      if (isBlocked)
      {
        return BadRequest(authBlockedMessage);
      }
    }

    bool verified = false;
    foreach (var verifier in userVerifiers)
    {
      verified = await verifier.Verify(user, password);
      if (verified)
      {
        break;
      }
    }

    if (!verified)
    {
      if (useAuthTimeout)
      {
        if (authTimeout.IncreaseTriesNumber(user))
        {
          return BadRequest(authBlockedMessage);
        }
      }

      return BadRequest(wrongCredsMessage);
    }

    var roles = await roleProvider.GetRoleNamesByUser(user);
    var payload = await tokenAuthenticator.Authenticate(user, roles)
        ?? throw new InvalidOperationException("Не удалось получить токен");
    return Ok(payload);
  }
}

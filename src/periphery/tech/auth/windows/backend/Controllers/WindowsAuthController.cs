namespace Tmobis.Auth.Windows.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tmobis.Auth;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Providers.Users;

[Route("/api/auth")]
[ApiController]
public class WindowsAuthController(
    IRoleProvider roleProvider,
    IEmployeeProvider employeeProvider,
    IUserProvider userProvider,
    ITokenAuthenticator tokenAuthenticator)
: ControllerBase
{
  [AllowAnonymous]
  [HttpGet("validate")]
  public async Task<IActionResult> Validate()
  {
    var identity = User?.Identity;
    if (identity == null || !identity.IsAuthenticated)
    {
      return Unauthorized("Пользователь не аутентифицирован");
    }

    string login = identity.Name?.Contains('\\') == true
        ? identity.Name.Split('\\')[1]
        : identity.Name ?? string.Empty;

    IUser? user = await userProvider.GetUserByLogin(login);
    IEmployee? employee = await employeeProvider.GetByUser(user);

    if (user == null || employee == null)
    {
      return BadRequest($"Пользователь не зарегистрирован в системе: {identity.Name}");
    }

    var roles = await roleProvider.GetRoleNamesByUser(user);
    var payload = await tokenAuthenticator.Authenticate(user, roles)
        ?? throw new InvalidOperationException("Не удалось получить токен");

    return Ok(JsonConvert.SerializeObject(payload));
  }
}

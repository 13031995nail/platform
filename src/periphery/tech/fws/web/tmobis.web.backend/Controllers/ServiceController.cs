namespace Tmobis.Frameworks.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tmobis.Frameworks.Web.Backend.Options;

[Route("api/Service")]
[ApiController]
public class ServiceController(
    IOptions<VersionOptions> versionOptions)
: ControllerBase
{
    private readonly VersionOptions versionOptions = versionOptions.Value;

    /// <summary>
    /// Возвращает последнюю версию интерфейса пользователя.
    /// </summary>
    /// <returns>Всегда <c>true</c>.</returns>
    [HttpGet("InterfaceVersion")]
    [AllowAnonymous]
    public string? InterfaceVersion()
    {
        return versionOptions?.InterfaceVersion;
    }
}

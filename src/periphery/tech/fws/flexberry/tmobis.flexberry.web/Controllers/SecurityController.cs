namespace Tmobis.Frameworks.Flexberry.Web.Controllers;
using System;
using System.Linq;
using System.Collections.Generic;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NewPlatform.Flexberry.Caching;
using Tmobis.Core.Entities;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.Flexberry.BusinessServers;

// TODO: вынести логику из контроллера в некий юзкейс
[Route("api/security")]
[ApiController]
[Authorize]
public class SecurityController(
    UserBS userBS,
    IEntityRegister entityRegister,
    [FromKeyedServices("DataServiceSecurity")] ISecurityManager securityManager,
    ICurrentUserProvider currentUserProvider,
    [FromKeyedServices("AgentCache")] ICacheService agentCache,
    [FromKeyedServices("SecurityCache")] ICacheService securityCache)
: ControllerBase
{
    private readonly UserBS userBS = userBS;
    private readonly IEntityRegister entityRegister = entityRegister;
    private readonly ISecurityManager securityManager = securityManager;
    private readonly ICurrentUserProvider currentUserProvider
    = currentUserProvider;

    /// <summary>
    /// Возвращает разрешенные для текущего пользователя
    /// действия над указанным классом.
    /// </summary>
    /// <param name="type">Класс, полномочия над
    /// которым надо проверить.</param>
    /// <returns>Список полномочий текущего пользователя
    /// над указанным классом.</returns>
    private List<string>? GetPermissionsForType(Type? type)
    {
        return type == null
        ? null
        : [.. Enum.GetValues(typeof(tTypeAccess))
              .Cast<tTypeAccess>()
              .Where(typeaccess => securityManager
                  .AccessObjectCheck(type, typeaccess, false))
              .Select(typeaccess => EnumCaption.GetCaptionFor(typeaccess))];
    }

    [Route("{operation}Object/{name}")]
    [HttpGet]
    public bool AccessObjectCheck(string operation, string name)
    {
        var type = entityRegister.GetTypeByName(name);
        return Enum.TryParse(operation, out tTypeAccess typeAccess)
        && securityManager.AccessObjectCheck(type, typeAccess, false);
    }

    [Route("GetPermissions/{name}")]
    [HttpGet]
    public List<string>? GetPermissions(string name)
    {
        var type = entityRegister.GetTypeByName(name);
        return GetPermissionsForType(type);
    }

    [Route("GetPermissions")]
    [HttpGet]
    public async Task<IDictionary<string, List<string>?>> GetPermissions()
    {
        var emptyResult = new Dictionary<string, List<string>?>();
        var user = await currentUserProvider.GetCurrentUser();
        if (user?.Id == null)
        {
            return emptyResult;
        }

        // Возвращается словарь, ключ в котором - наименование типа класса,
        // значение - коллекция доступных для текущего пользователя
        // действий над данным классом.
        var objectTypeNames = userBS.GetAllPermissions(user.Id)
            .Distinct()
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
        return objectTypeNames
            .ToDictionary(
                x => x,
                entityRegister.GetTypeByName)
            .Where(x => x.Value != null)
            .ToDictionary(
                x => x.Key,
                x => GetPermissionsForType(x.Value));
    }

    [Route("ClearPermitionsCache")]
    [HttpPost]
    public void ClearPermitionsCache()
    {
        securityCache.ClearCache();
        agentCache.ClearCache();
    }
}

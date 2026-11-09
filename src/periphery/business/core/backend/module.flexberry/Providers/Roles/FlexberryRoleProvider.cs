namespace Tmobis.Business.Core.Flexberry.Providers.Roles;
using System;
using System.Collections.Generic;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.Business.LINQProvider;
using ICSSoft.STORMNET.Security;
using ICSSoft.STORMNET.FunctionalLanguage;
using Microsoft.Extensions.Logging;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Frameworks.Flexberry.Linq;
using DataObject = ICSSoft.STORMNET.DataObject;

/// <summary>
/// Сервис получения ролей на основе Flexberry.
/// </summary>
public class FlexberryRoleProvider(
  IDataService dataService,
  ILogger<FlexberryRoleProvider> logger) : EmptyRoleProvider
{
  private readonly IDataService dataService = dataService;
  private readonly ILogger<FlexberryRoleProvider> logger = logger;

  /// <inheritdoc/>
  public override ValueTask<List<string>> GetRoleNamesByUser(IUser? user)
  {
    try
    {
      var userPk = user.Id.ToString();
      var selfRoles = dataService
        .Query<LinkRole>(LinkRole.Views.Sec_GetRoles)
        .Where(link => link.Agent.__PrimaryKey.Equals(userPk))
        .ToList()
        .Select(link => link.Role)
        .ToList();
      var roles = GetRolesWithParentOnes(selfRoles);
      return ValueTask.FromResult<List<string>>([.. roles.Select(x => x.Name)]);
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Ошибка при получении ролей пользователя");
      return ValueTask.FromResult<List<string>>([]);
    }
  }

  public override async ValueTask<bool> HasRole(IUser? user, string roleName)
  {
    var roles = await GetRoleNamesByUser(user);
    return roles.Contains(roleName);
  }

  public override async Task GrantRole(IUser user, string roleName)
  {
    if (user is not Models.User flexberryUser)
    {
      return;
    }

    if (await HasRole(user, roleName))
    {
      return;
    }

    var role = GetRoleByName(roleName);
    if (role == null)
    {
      return;
    }

    var linkRole = PKHelper.CreateDataObject<LinkRole>(Guid.NewGuid());
    linkRole.Agent = flexberryUser;
    linkRole.Role = role;
    linkRole.SetStatus(ObjectStatus.Created);
    linkRole.AddLoadedProperties(nameof(LinkRole.Agent), nameof(LinkRole.Role));

    DataObject linkRoleDataObject = linkRole;
    dataService.UpdateObject(ref linkRoleDataObject);
  }

  /// <summary>
  /// Получение всех ролей, включая родительские, на базе списка ролей.
  /// </summary>
  /// <param name="roles">Роли, чьи родительские роли нужно найти.</param>
  /// <returns>Все найденные роли (включая исходные).</returns>
  private List<Agent> GetRolesWithParentOnes(List<Agent> roles)
  {
    if (roles == null || roles.Count == 0)
    {
      return [];
    }

    // TODO: Переписать, разобраться в логике
    var langDef = ICSSoft.STORMNET.Windows.Forms.ExternalLangDef.LanguageDef;
    LoadingCustomizationStruct lcs = LoadingCustomizationStruct
      .GetSimpleStruct(typeof(LinkRole), LinkRole.Views.GetParentRoles);

    // Условие, что и дочерний, и родительский элемент - это роли.
    var baseCheckFunction = langDef.GetFunction(
        langDef.funcAND,
        langDef.GetFunction(langDef.funcEQ,
          new VariableDef(langDef.BoolType, "Agent.IsRole"), langDef.paramTrue),
        langDef.GetFunction(langDef.funcEQ,
          new VariableDef(langDef.BoolType, "Role.IsRole"), langDef.paramTrue));
    List<Agent> resultList = [.. roles];
    List<object> rolesId = [.. roles.Select(x => x.__PrimaryKey)];
    List<object> newRolesId = [.. rolesId.Select(x => x)];
    while (newRolesId.Count > 0)
    {
      /*
       * Данный цикл не зациклится, поскольку из newRolesId
       * исключаются все ранее встреченные роли.
       * Соответственно, если в ролях был цикл, то при возвращении
       * в вершину, из которой шло рассмотрение, повторно вершина
       * рассматриваться не будет.
       */

      var agentList = new List<object>()
      { new VariableDef(langDef.GuidType, "Agent") };
      agentList.AddRange(newRolesId);

      var roleList = new List<object>()
      { new VariableDef(langDef.GuidType, "Role") };
      roleList.AddRange(rolesId);

      /*
       * Ограничение следующее:
       * дочерний и родительский элемент - это роли,
       * идентификатор дочернего элемента роли содержится в текущем списке поиска,
       * идентификатор родительского элемента роли не содержится
       * в списке идентификаторов уже найденных ролей.
       */
      lcs.LimitFunction =
          langDef.GetFunction(
              langDef.funcAND,
              baseCheckFunction,
              langDef.GetFunction(langDef.funcIN, [.. agentList]),
              langDef.GetFunction(langDef.funcNOT,
                langDef.GetFunction(langDef.funcIN, [.. roleList])));
      List<LinkRole> newRoles = [.. dataService
        .LoadObjects(lcs).Cast<LinkRole>()];
      newRolesId = [.. newRoles.Select(x => x.Role.__PrimaryKey)];
      rolesId.AddRange(newRolesId);
      resultList.AddRange([.. newRoles.Select(x => x.Role)]);
    }

    return resultList;
  }

  private Role? GetRoleByName(string roleName)
  {
    var roleView = new View
    {
      DefineClassType = typeof(Role),
    };
    roleView.AddProperty(nameof(Role.Name));
    roleView.AddProperty(nameof(Role.IsRole));

    return dataService.QueryExt<Role>(roleView)
      .FirstOrDefault(x => x.IsRole && x.Name == roleName);
  }
}

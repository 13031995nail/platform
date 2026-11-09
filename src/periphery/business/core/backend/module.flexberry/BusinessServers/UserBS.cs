namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using System;
using System.Collections.Generic;
using System.Linq;
using ICSSoft.STORMNET;
using ICSSoft.STORMNET.Business;
using ICSSoft.STORMNET.FunctionalLanguage;
using ICSSoft.STORMNET.FunctionalLanguage.SQLWhere;
using ICSSoft.STORMNET.Security;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Cache;
using User = Models.User;

[ICSSoft.STORMNET.AccessType(ICSSoft.STORMNET.AccessType.none)]
public class UserBS(ICachedEntityProvider<IUser> userCache)
: ICSSoft.STORMNET.Business.BusinessServer
{
  private readonly ICachedEntityProvider<IUser> userCache = userCache;

  /// <summary>
  /// Получить все субъекты, на которые есть права у заданного пользователя.
  /// </summary>
  /// <param name="userKey">Пользователь, для которого требуется
  /// получение доступных субъектов.</param>
  public List<string> GetAllPermissions(Guid userKey)
  {
    List<Guid> agentPKs = new List<Guid>();

    PrvAddLinkedAgents(userKey, ref agentPKs);

    SQLWhereLanguageDef langdef = SQLWhereLanguageDef.LanguageDef;
    object[] funcInParams = new object[agentPKs.Count + 1];
    funcInParams[0] = new VariableDef(langdef.GuidType, "Agent");
    for (int i = 0; i < agentPKs.Count; i++)
    {
      funcInParams[i + 1] = agentPKs[i];
    }

    Function lfPermit = langdef.GetFunction(langdef.funcIN, funcInParams);
    View v = new View();
    v.DefineClassType = typeof(Permition);
    v.AddProperty("Agent");
    v.AddProperty("Subject.Name");

    var lcsPermit = LoadingCustomizationStruct
      .GetSimpleStruct(typeof(Permition), v);
    lcsPermit.LimitFunction = lfPermit;

    var permits = DataService.LoadObjects(lcsPermit);
    var subjects = new List<string>(permits.Length);
    subjects.AddRange(permits
        .Cast<Permition>()
        .Where(permit => permit.Subject != null)
        .Select(permit => permit.Subject.Name));
    return subjects;
  }

  /// <summary>
  /// Осуществляет добавление к пользователю прилинкованных агентов.
  /// </summary>
  /// <param name="userKey">Ключ пользователя.</param>
  /// <param name="agentPKs">Ключи агентов.</param>
  private void PrvAddLinkedAgents(Guid userKey, ref List<Guid> agentPKs)
  {
    // Чтобы избежать циклической обработки, если вдруг ссылки будут стоять друг на друга.
    if (!agentPKs.Contains(userKey))
    {
      agentPKs.Add(userKey);

      // Сначала проверим ссылки.
      SQLWhereLanguageDef langdef = SQLWhereLanguageDef.LanguageDef;

      // Группы.
      Function lfLinkGroup = langdef.GetFunction(langdef.funcEQ, new VariableDef(langdef.GuidType, "User"), userKey);
      LoadingCustomizationStruct lcsLinkGroup = LoadingCustomizationStruct.GetSimpleStruct(typeof(LinkGroup), "LinkedAgents");
      lcsLinkGroup.LimitFunction = lfLinkGroup;

      ICSSoft.STORMNET.DataObject[] objsLinkGroup = DataService.LoadObjects(lcsLinkGroup);

      // Рекурсивно обработаем найденные группы.
      foreach (LinkGroup link in objsLinkGroup)
      {
        PrvAddLinkedAgents(((ICSSoft.STORMNET.KeyGen.KeyGuid)link.Group.__PrimaryKey).Guid, ref agentPKs);
      }

      // Роли.
      Function lfLinkRole = langdef.GetFunction(langdef.funcEQ, new VariableDef(langdef.GuidType, "Agent"), userKey);

      // Нужно добавить ограничение на время действия роли.
      LoadingCustomizationStruct lcsLinkRole = LoadingCustomizationStruct.GetSimpleStruct(typeof(LinkRole), "LinkedAgents");
      lcsLinkRole.LimitFunction = lfLinkRole;
      ICSSoft.STORMNET.DataObject[] objsLinkRole = DataService.LoadObjects(lcsLinkRole);

      // Рекурсивно обработаем найденные роли.
      foreach (LinkRole link in objsLinkRole.Cast<LinkRole>())
      {
        PrvAddLinkedAgents(((ICSSoft.STORMNET.KeyGen.KeyGuid)link.Role.__PrimaryKey).Guid, ref agentPKs);
      }
    }

    if (!agentPKs.Contains(Agent.AllUsersRoleGuid))
    {
      agentPKs.Add(Agent.AllUsersRoleGuid);
    }
  }

  public virtual ICSSoft.STORMNET.DataObject[] OnUpdateUser(User UpdatedObject)
  {
    if (UpdatedObject.GetStatus() != ICSSoft.STORMNET.ObjectStatus.Created)
    {
      var notAllowedProps = new[]
      {
        "Login", "IsUser", "IsGroup", "IsRole", "ConnString", "Town"
      };

      var badProps = UpdatedObject.GetAlteredPropertyNames().Intersect(notAllowedProps);

      if (badProps.Any())
      {
        throw new Exception("Запрещено изменять поля: " + string.Join(",", badProps));
      }
    }

    if (UpdatedObject.IsUser)
    {
      userCache.RemoveFromCache(UpdatedObject.Login);
    }

    var user = UpdatedObject as User;
    if (user.IsAlteredProperty(nameof(User.UserPwd)))
    {
      user.PwdEditTime = DateTime.Now;

      if (user.GetStatus() != ObjectStatus.Created)
      {
        User previousUser = (User)user.GetDataCopy();
        user.Pwd2 = previousUser.Pwd1;
        user.Pwd1 = previousUser.UserPwd;
      }
    }

    if (user.IsAlteredProperty(nameof(User.UserPwd)))
    {
      user.PwdEditTime = DateTime.Now;
    }

    return [];
  }
}

namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using ICSSoft.STORMNET;
using System;
using System.Linq;
using DataObject = ICSSoft.STORMNET.DataObject;
using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Core.Models.Towns;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.Flexberry.Models;

[ICSSoft.STORMNET.AccessType(ICSSoft.STORMNET.AccessType.none)]
public class LocalDictionaryBS(ICurrentUserProvider currentUserProvider)
: ICSSoft.STORMNET.Business.BusinessServer
{

  private readonly ICurrentUserProvider currentUserProvider = currentUserProvider;

  /// <summary>
  /// Проверяет, зависит ли определённый тип от города.
  /// </summary>
  /// <param name="type">Тип.</param>
  public virtual bool IsLocalDictionary(Type type)
  {
    return type.GetInterfaces().Contains(typeof(IWTown));
  }

  public virtual ICSSoft.STORMNET.DataObject[] OnUpdateIFlexberryWTown(IFlexberryWTown UpdatedObject)
  {
    if (UpdatedObject is DataObject updObject)
    {
      if (UpdatedObject.Town == null && (updObject.GetStatus() == ObjectStatus.Created || updObject.CheckLoadedProperty("Town")))
      {
        IUser currentUser = GetCurrentUser();
        if (currentUser != null && currentUser.Town != null)
        {
          UpdatedObject.Town = currentUser.Town;
        }
      }
    }

    return [];
  }

  private IUser? GetCurrentUser()
  {
      var currentUserTask = currentUserProvider.GetCurrentUser();
      currentUserTask.Wait();
      return currentUserTask.Result;
  }
}

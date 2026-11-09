namespace Tmobis.Flexberry.Autoincrement.BusinessServers;

using ICSSoft.STORMNET;
using Tmobis.Flexberry.Autoincrement.Objects;

[AccessType(AccessType.none)]
public class AutoincrementedBS(IAutoincrementService autoincrementService)
: ICSSoft.STORMNET.Business.BusinessServer
{
  private readonly IAutoincrementService autoincrementService = autoincrementService;

  public virtual DataObject[] OnUpdateIAutoincremented(IAutoincremented UpdatedObject)
  {
    if (UpdatedObject.AutoincrementMode == AutoincrementMode.Automatic)
    {
      var dObject = UpdatedObject as DataObject;
      if (dObject == null || dObject.GetStatus() == ObjectStatus.Created)
      {
        UpdatedObject.SetAutoincrementedFieldValue(autoincrementService.GetNext(UpdatedObject));
      }
    }

    return [];
  }
}

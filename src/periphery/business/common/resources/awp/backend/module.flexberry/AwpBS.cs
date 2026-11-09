namespace Tmobis.Business.Common.Resources;

using ICSSoft.STORMNET;
using DataObject = ICSSoft.STORMNET.DataObject;

[AccessType(AccessType.@this)]
public class AwpBS
: ICSSoft.STORMNET.Business.BusinessServer
{
  public virtual DataObject[] OnUpdateAwp(Awp UpdatedObject)
  {
    if (UpdatedObject is DataObject updObject)
    {
      var status = updObject.GetStatus();
      if (status == ObjectStatus.Created || status == ObjectStatus.Altered)
      {
        if ((updObject.IsAlteredProperty(nameof(UpdatedObject.Actuality)) || updObject.IsAlteredProperty(nameof(UpdatedObject.IsCurrent)))
          && !UpdatedObject.Actuality && UpdatedObject.IsCurrent)
        {
          UpdatedObject.IsCurrent = false;
        }
        if (updObject.IsAlteredProperty(nameof(UpdatedObject.DateBegin)) || updObject.IsAlteredProperty(nameof(UpdatedObject.DateEnd)))
        {
            var name = string.Format("{0}-{1}", UpdatedObject.DateBegin.Year, UpdatedObject.DateEnd.Year);
            UpdatedObject.Name = name;
        }
      }
    }

    return [];
  }
}

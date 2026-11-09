namespace Tmobis.Business.Common.Contacts;

using ICSSoft.STORMNET;
using DataObject = ICSSoft.STORMNET.DataObject;

[AccessType(AccessType.@this)]
public class OrganizationBS
: ICSSoft.STORMNET.Business.BusinessServer
{
    public virtual DataObject[] OnUpdateOrganization(Organization UpdatedObject)
    {
        if (UpdatedObject is DataObject updObject)
        {
            var status = updObject.GetStatus();
            if (status == ObjectStatus.Created || status == ObjectStatus.Altered)
            {
                if (updObject.IsAlteredProperty(nameof(UpdatedObject.Hierarchy)))
                {
                    var name = UpdatedObject.Hierarchy?.Name;
                    UpdatedObject.HierarchyName = name;
                }
            }
        }

        return [];
    }
}

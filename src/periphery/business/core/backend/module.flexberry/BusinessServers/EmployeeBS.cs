namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Core.Medium.Producers;
using DataObject = ICSSoft.STORMNET.DataObject;

[ICSSoft.STORMNET.AccessType(ICSSoft.STORMNET.AccessType.none)]
public class EmployeeBS(IMessageDispatcher messageDispatcher)
: MessageBasedEntityBS<Employee>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateEmployee(Employee UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];
}

namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;

public class OnEmployeeUpdated(EmployeeService service)
: OnEntityUpdated<EntityUpdated<Employee>, Employee>(null)
{
    private readonly EmployeeService service = service;
    protected override async ValueTask<List<IEntity>> ChangeEntity(Employee entity)
    {
        await service.EnsureOnlyEmployeeByUser(entity);
        return [];
    }
}

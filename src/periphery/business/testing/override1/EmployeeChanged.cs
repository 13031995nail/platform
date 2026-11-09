namespace Tmobis.Business.Override1;

using Tmobis.Core.Entities.Consumers;using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class EmployeeChanged(IRepository? repository)
    : OnEntityUpdated<EntityUpdated<IEmployeeWType>, IEmployeeWType>(repository)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(IEmployeeWType entity)
    {
        entity.Email = "YAY";
        return [];
    }
}

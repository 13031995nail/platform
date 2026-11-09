namespace Tmobis.Business.Override2;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class EmployeeChanged(IRepository? repository)
    : OnEntityUpdated<EntityUpdated<IEmployeeWBirthDate>, IEmployeeWBirthDate>(repository)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(IEmployeeWBirthDate entity)
    {
        entity.BirthDate = DateTime.UtcNow;
        return [];
    }
}

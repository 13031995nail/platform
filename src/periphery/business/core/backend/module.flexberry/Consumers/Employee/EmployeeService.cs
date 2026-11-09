namespace Tmobis.Business.Core.Flexberry.BusinessServers;

using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Core.Repositories;

public class EmployeeService(IEmployeeProvider employeeProvider)
{
    public async Task EnsureOnlyEmployeeByUser(Employee entity)
    {
        if (entity.User != null && entity.IsAlteredProperty(nameof(entity.User)))
        {
            var existingEmployee = await employeeProvider.GetByUser(entity.User);
            if (existingEmployee != null && existingEmployee.Id != entity.Id)
            {
                throw new InvalidOperationException(
                    "Данный пользователь уже назначен другому сотруднику");
            }
        }
    }
}

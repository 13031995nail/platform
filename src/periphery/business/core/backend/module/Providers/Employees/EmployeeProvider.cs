namespace Tmobis.Business.Core.Providers.Employees;

using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Core.Repositories;

/// <inheritdoc/>
public class EmployeeProvider(IReadRepository repository)
: IEmployeeProvider
{
    /// <inheritdoc/>
    public virtual async Task<IEmployee?> GetById(Guid id)
    => await repository.GetById<IEmployee>(id);

    /// <inheritdoc/>
    public virtual async Task<IEmployee?> GetByUser(IUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return await repository.FirstOrDefault<IEmployee>(x =>
            x.Actuality && x.User!.Id == user.Id);
    }

    /// <inheritdoc/>
    public virtual Task<List<IEmployee>> GetWithPhone()
        => repository.Find<IEmployee>(employee =>
            employee.Actuality && employee.Phone != null);
}

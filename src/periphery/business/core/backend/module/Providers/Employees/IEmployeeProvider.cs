namespace Tmobis.Business.Core.Providers.Employees;
using Tmobis.Business.Core.Models.Employees;
using Tmobis.Business.Core.Models.Users;

/// <summary>
/// Сервис получения сотрудников.
/// </summary>
public interface IEmployeeProvider
{
    /// <summary>
    /// Поиск сотрудника по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сотрудника.</param>
    public Task<IEmployee?> GetById(Guid id);

    /// <summary>
    /// Поиск сотрудника по соответствующему ему пользователю.
    /// </summary>
    /// <param name="user">Сущность пользователя.</param>
    public Task<IEmployee?> GetByUser(IUser user);

    /// <summary>
    /// Поиск действующих сотрудников с указанным номером телефона.
    /// </summary>
    public Task<List<IEmployee>> GetWithPhone();
}

namespace Tmobis.Integrations.Messengers.Providers;

using Tmobis.Business.Core.Models.Employees;
using Tmobis.Integrations.Messengers.Models;

public interface IEmployeeMessengerProvider
{
    Task<IEmployeeMessenger?> GetByExternalId(
        string messengerCode,
        string externalId);

    Task<IEmployeeMessenger?> GetByEmployee(
        Guid employeeId,
        string messengerCode);

    Task<IEmployeeMessenger?> GetPreferred(Guid employeeId);

    Task<IEmployeeMessenger> Link(
        IEmployee employee,
        string messengerCode,
        string externalId);

    Task SetPreferredMessengerIfMissing(IEmployeeMessenger link);
}

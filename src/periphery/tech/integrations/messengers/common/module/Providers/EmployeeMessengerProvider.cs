namespace Tmobis.Integrations.Messengers.Providers;

using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Repositories;
using Tmobis.Integrations.Messengers.Models;

public sealed class EmployeeMessengerProvider(IRepository repository)
    : IEmployeeMessengerProvider
{
    public async Task<IEmployeeMessenger?> GetByExternalId(
        string messengerCode,
        string externalId)
    {
        var messenger = await GetMessenger(messengerCode);
        return messenger == null
            ? null
            : await repository.FirstOrDefault<IEmployeeMessenger>(link =>
                link.MessengerId == messenger.Id
                && link.ExternalId == externalId);
    }

    public async Task<IEmployeeMessenger?> GetByEmployee(
        Guid employeeId,
        string messengerCode)
    {
        var messenger = await GetMessenger(messengerCode);
        return messenger == null
            ? null
            : await repository.FirstOrDefault<IEmployeeMessenger>(link =>
                link.EmployeeId == employeeId
                && link.MessengerId == messenger.Id);
    }

    public Task<IEmployeeMessenger?> GetPreferred(Guid employeeId)
        => repository.FirstOrDefault<IEmployeeMessenger>(link =>
            link.EmployeeId == employeeId && link.IsPreferred);

    public async Task<IEmployeeMessenger> Link(
        IEmployee employee,
        string messengerCode,
        string externalId)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ArgumentException.ThrowIfNullOrWhiteSpace(messengerCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);

        var messenger = await GetMessenger(messengerCode)
            ?? throw new InvalidOperationException(
                $"Мессенджер с кодом '{messengerCode}' не найден.");
        var link = await repository.FirstOrDefault<IEmployeeMessenger>(item =>
            item.EmployeeId == employee.Id
            && item.MessengerId == messenger.Id);
        if (link == null)
        {
            link = repository.MakeEntity<IEmployeeMessenger>();
            link.Employee = employee;
            link.EmployeeId = employee.Id;
            link.Messenger = messenger;
            link.MessengerId = messenger.Id;
            repository.Create(link);
        }

        link.ExternalId = externalId;
        return link;
    }

    public async Task SetPreferredMessengerIfMissing(
        IEmployeeMessenger link)
    {
        ArgumentNullException.ThrowIfNull(link);
        if (await GetPreferred(link.EmployeeId) != null)
        {
            return;
        }

        link.IsPreferred = true;
    }

    private Task<IMessenger?> GetMessenger(string messengerCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messengerCode);
        return repository.FirstOrDefault<IMessenger>(messenger =>
            messenger.Actuality && messenger.Code == messengerCode);
    }
}

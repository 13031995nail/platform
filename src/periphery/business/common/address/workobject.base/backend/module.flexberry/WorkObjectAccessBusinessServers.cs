namespace Tmobis.Business.Common.Address;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public sealed class WorkObjectBS(IWorkObjectAccessProvider accessProvider)
    : EntityBS<WorkObject>
{
    public DataObject[] OnUpdateWorkObject(WorkObject updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(WorkObject entity)
    {
        if (entity.GetStatus() == ObjectStatus.Created)
        {
            await accessProvider.EnsureCanCreateWorkObject(entity.Organization?.Id ?? Guid.Empty);
        }
        else
        {
            await accessProvider.EnsureCanAccess(entity.Id);
            if (entity.GetAlteredPropertyNames().Contains(nameof(WorkObject.Organization)))
            {
                await accessProvider.EnsureCanCreateWorkObject(
                    entity.Organization?.Id ?? Guid.Empty);
            }
        }

        return await base.OnEntityChangedAsync(entity);
    }
}

public sealed class WorkObjectTsoOrganizationBS(
    IWorkObjectAccessProvider accessProvider,
    IRepository repository)
    : EntityBS<WorkObjectTsoOrganization>
{
    public DataObject[] OnUpdateWorkObjectTsoOrganization(
        WorkObjectTsoOrganization updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        WorkObjectTsoOrganization entity)
    {
        await accessProvider.EnsureTechnicalAdministrator();
        if (entity.GetStatus() != ObjectStatus.Deleted)
        {
            var organizationId = entity.Organization?.Id;
            if (!organizationId.HasValue && entity.Id != Guid.Empty)
            {
                var storedLinks = await repository.Find<WorkObjectTsoOrganization>(link =>
                    link.Id == entity.Id);
                organizationId = storedLinks.FirstOrDefault()?.Organization?.Id;
            }

            var isTsoOrganization = organizationId.HasValue
                && await repository.Any<Organization>(organization =>
                    organization.Id == organizationId.Value
                    && organization.OrganizationType.Id == OrganizationType.Keys.Tco);
            if (!isTsoOrganization)
            {
                throw new InvalidOperationException(
                    "Связать объект можно только с организацией типа ТСО.");
            }
        }

        return await base.OnEntityChangedAsync(entity);
    }
}

namespace Tmobis.Business.Process.Documents;

using Common.Contacts;
using Core.Providers.Employees;
using Base;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnBaseActCreated(
  IActNumberBuilder actNumberBuilder,
  IRepository repository,
  IEmployeeProvider employeeProvider)
  : OnBaseActCreatedBase(actNumberBuilder, repository, employeeProvider)
{
  protected override async ValueTask PrepareEntity(
    IBaseActBase entity)
  {
    // TODO: Придумать получше способ инициализации нужных свойств
    // для бизнес-операций при апдейте
    if (entity is IBaseAct baseAct && baseAct.Organization != null)
    {
      var organization = await repository.GetById<IOrganizationBase>(
        baseAct.Organization.Id);

      if (organization != null)
      {
        baseAct.Organization = organization;
      }
    }
  }

  protected override async ValueTask<List<IEntity>> ChangeEntity(
    IBaseActBase entity)
  {
    var toUpdate = await base.ChangeEntity(entity);

    // TODO: Придумать получше способ обработки бизнес-операций при апдейте
    if (entity is IBaseAct baseAct && baseAct.Organization != null)
    {
      var organization = await repository.GetById<IOrganizationBase>(
        baseAct.Organization.Id);

      if (organization != null)
      {
        baseAct.OrganizationName = organization.Name;
        baseAct.OrganizationPhone = organization.Phone;
        baseAct.OrganizationAddress = organization.Address;
      }
    }

    return toUpdate;
  }
}
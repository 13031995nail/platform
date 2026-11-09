namespace Tmobis.Business.Process.Documents.Base;

using Core.Providers.Employees;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnBaseActCreatedBase(
  IActNumberBuilder actNumberBuilder,
  IRepository repository,
  IEmployeeProvider employeeProvider)
: OnEntityUpdated<IEntityCreated<IBaseActBase>, IBaseActBase>(null)
{
  protected override async ValueTask PrepareEntity(
  IBaseActBase entity)
  {
    // TODO: Придумать получше способ инициализации нужных свойств
    // для бизнес-операций при апдейте
    if (entity.ActType != null)
    {
      var actType = await repository.GetById<IActType>(entity.ActType.Id);
      if (actType != null)
      {
        entity.ActType = actType;
      }
    }

    if (entity.Assignee != null)
    {
      var assignee = await employeeProvider.GetById(entity.Assignee.Id);
      if (assignee != null)
      {
        entity.Assignee = assignee;
      }
    }
  }

  protected override async ValueTask<List<IEntity>> ChangeEntity(
    IBaseActBase entity)
  {
    SetDocumentNumber(entity);
    await SetDocumentStatus(entity);
    return [];
  }

  protected virtual void SetDocumentNumber(IBaseActBase document)
  {
    document.ActNumber = actNumberBuilder.GetActNumber(document);
  }

  protected virtual async ValueTask SetDocumentStatus(IBaseActBase document)
  {
    document.Status = await repository.GetById<IActStatus>(IActStatus.Keys.Editing);
  }
}

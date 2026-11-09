namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Common.Contacts;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Medium.Producers;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnEvaluationSheetUpdated(
    IMessageDispatcher messageDispatcher,
    IRepository repository,
    EvaluationSheetService service)
  : OnEntityUpdated<EntityUpdated<EvaluationSheet>, EvaluationSheet>(null)
{
    protected override async ValueTask PrepareEntity(EvaluationSheet entity)
    {
        if (!(entity == null || entity.Organization == null))
        {
            var organization = await repository.GetById<Organization>(entity.Organization.Id);
            var key = organization?.OrganizationType.Id;
            var recount = service.RecountObjectReadinessLevel(key, entity);
            entity.ObjectReadinessIndex = (decimal)recount;
            entity.ObjectReadinessLevel = service.GetStatus(recount, key, entity);
        }
    }

    protected override async ValueTask<List<IEntity>> ChangeEntity(EvaluationSheet entity)
    {
        var changesAttrs = entity.GetAlteredPropertyNames();
        var updatedEntities = new List<IEntity>();
        var status = Information.ExtractPropertyName<EvaluationSheet>(f => f.ActStatus);
        if (changesAttrs.Contains(status) && entity.ActStatus.Id == IActStatus.Keys.Signed)
        {
            EvaluationSheetSigned msg = new EvaluationSheetSigned() { Act = entity };
            var result = await messageDispatcher.SendWithResult<EvaluationSheetSigned, IEntityUpdatedResult>(msg);
            if (result != null)
            {
                updatedEntities.AddRange(result.EntitiesToUpdate);
            }
        }
        return updatedEntities;
    }
}

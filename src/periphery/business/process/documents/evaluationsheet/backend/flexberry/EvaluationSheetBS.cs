namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using System.Linq;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Medium.Producers;

public class EvaluationSheetBS(
    IMessageDispatcher messageDispatcher,
    EvaluationSheetAccess access)
    : MessageBasedEntityBS<EvaluationSheet>(messageDispatcher)
{
    public virtual DataObject[] OnUpdateEvaluationSheet(
        EvaluationSheet UpdatedObject)
    => [.. OnEntityChanged(UpdatedObject).Cast<DataObject>()];

    protected override async Task<List<Tmobis.Core.Models.IEntity>> OnEntityChangedAsync(
        EvaluationSheet entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}

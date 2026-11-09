namespace Tmobis.Business.Process.Documents;

using ICSSoft.STORMNET;
using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;

public sealed class EvaluationSheetRequirementBS(EvaluationSheetAccess access)
    : EntityBS<EvaluationSheetRequirement>
{
    public DataObject[] OnUpdateEvaluationSheetRequirement(
        EvaluationSheetRequirement updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        EvaluationSheetRequirement entity)
    {
        await access.EnsureCanChange(entity);
        if (entity.GetAlteredPropertyNames(true).Intersect(new[]
            {
                nameof(EvaluationSheetRequirement.CalculatedReq),
                nameof(EvaluationSheetRequirement.HasEnteredValue),
                nameof(EvaluationSheetRequirement.SupportingDocFile)
            }).Any())
        {
            entity.IsVerified = false;
        }
        return await base.OnEntityChangedAsync(entity);
    }
}

public sealed class EvaluationSheetSignBS(EvaluationSheetAccess access)
    : EntityBS<EvaluationSheetSign>
{
    public DataObject[] OnUpdateEvaluationSheetSign(EvaluationSheetSign updatedObject)
        => [.. OnEntityChanged(updatedObject).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        EvaluationSheetSign entity)
    {
        await access.EnsureCanChange(entity);
        return await base.OnEntityChangedAsync(entity);
    }
}

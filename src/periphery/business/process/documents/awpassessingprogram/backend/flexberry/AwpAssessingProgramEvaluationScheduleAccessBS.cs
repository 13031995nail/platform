namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Core.Flexberry.BusinessServers;
using Tmobis.Core.Models;
using DataObject = ICSSoft.STORMNET.DataObject;

public sealed class AwpAssessingProgramEvaluationScheduleAccessBS(AwpAssessingProgramAccess access)
    : EntityBS<AwpAssessingProgramEvaluationSchedule>
{
    public DataObject[] OnUpdateAwpAssessingProgramEvaluationSchedule(
        AwpAssessingProgramEvaluationSchedule entity)
        => [.. OnEntityChanged(entity).Cast<DataObject>()];

    protected override async Task<List<IEntity>> OnEntityChangedAsync(
        AwpAssessingProgramEvaluationSchedule entity)
    {
        await access.EnsureCanChange(entity);
        return [];
    }
}

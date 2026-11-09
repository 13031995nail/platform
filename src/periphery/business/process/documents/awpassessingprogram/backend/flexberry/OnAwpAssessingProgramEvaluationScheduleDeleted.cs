namespace Tmobis.Business.Process.Documents;

using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAwpAssessingProgramEvaluationScheduleDeleted(IRepository repository)
: OnEntityUpdated<EntityDeleted<AwpAssessingProgramEvaluationSchedule>, AwpAssessingProgramEvaluationSchedule>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AwpAssessingProgramEvaluationSchedule entity)
    {
        var toUpdate = new List<IEntity>();
        toUpdate.AddRange(await DeleteObjects(entity));
        return toUpdate;
    }

    protected virtual async Task<List<AwpAssessingProgramObject>>
     DeleteObjects(AwpAssessingProgramEvaluationSchedule entity)
    {
        var programId = entity?.Program?.Id;
        var objectFunctionId = entity?.ObjectFunction?.Id;
        var objects = await repository.Find<AwpAssessingProgramObject>(programObject
            => programObject.WorkObject.ObjectFunction.Id.Equals(objectFunctionId)
               && programObject.Program.Id.Equals(programId));
        objects.ForEach(programObject
            => programObject.SetStatus(ICSSoft.STORMNET.ObjectStatus.Deleted));

        return objects;
    }
}

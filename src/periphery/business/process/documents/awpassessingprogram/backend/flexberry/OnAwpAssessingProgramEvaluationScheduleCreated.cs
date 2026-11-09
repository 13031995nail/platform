namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Address;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAwpAssessingProgramEvaluationScheduleCreated(IRepository repository)
: OnEntityUpdated<EntityCreated<AwpAssessingProgramEvaluationSchedule>, AwpAssessingProgramEvaluationSchedule>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AwpAssessingProgramEvaluationSchedule entity)
    {
        var toUpdate = new List<IEntity>();
        toUpdate.AddRange(await CreateObjects(entity));
        return toUpdate;
    }

    protected virtual async Task<List<AwpAssessingProgramObject>>
    CreateObjects(AwpAssessingProgramEvaluationSchedule entity)
    {
        var objects = new List<AwpAssessingProgramObject>();
        var orgId = entity?.Program?.Organization?.Id;
        var objFuncId = entity?.ObjectFunction?.Id;
        var orgObjects = await repository
        .Find<WorkObject>(obj =>
            (obj.Organization.Id.Equals(orgId)
             || (obj.Organization.Hierarchy != null &&
                 obj.Organization.Hierarchy.Id.Equals(orgId)))
            && obj.ObjectFunction.Id.Equals(objFuncId));

        foreach (var orgObject in orgObjects)
        {
            var programObject = repository.MakeEntity<AwpAssessingProgramObject>();
            programObject.WorkObject = orgObject;
            programObject.Program = entity?.Program;
            repository.Create(programObject);
            objects.Add(programObject);
        }

        entity.ObjectsCount = objects.Count;

        return objects;
    }
}

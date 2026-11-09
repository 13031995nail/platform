namespace Tmobis.Business.Process.Documents;

using Common.Address;
using Common.Personnel;
using Flexberry;
using Tmobis.Core.Entities;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnAwpAssessingProgramCreated(IRepository repository)
: OnEntityUpdated<EntityCreated<AwpAssessingProgram>, AwpAssessingProgram>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(AwpAssessingProgram entity)
    {
        var toUpdate = new List<IEntity>();
        toUpdate.AddRange(await CreateSchedules(entity));
        toUpdate.AddRange(await CreateSigns(entity));
        return toUpdate;
    }

    protected virtual async Task<List<AwpAssessingProgramEvaluationSchedule>>
    CreateSchedules(AwpAssessingProgram entity)
    {
        var schedules = new List<AwpAssessingProgramEvaluationSchedule>();
        var objectFunctions = await repository.All<ObjectFunction>();

        foreach (var objectFunction in objectFunctions)
        {
            var schedule = repository.MakeEntity<AwpAssessingProgramEvaluationSchedule>();
            schedule.ObjectFunction = objectFunction;
            schedule.Program = entity;

            var orgId = entity.Organization?.Id;
            var orgObjects = await repository
            .Find<WorkObject>(obj =>
                (obj.Organization.Id.Equals(orgId)
                    || (obj.Organization.Hierarchy != null &&
                        obj.Organization.Hierarchy.Id.Equals(orgId)))
                && obj.ObjectFunction.Id.Equals(objectFunction.Id));

            schedule.ObjectsCount = orgObjects.Count;
            repository.Create(schedule);
            schedules.Add(schedule);
        }

        return schedules;
    }

    /// <summary>
    /// При создании программы автоматически в подписанты добавляется Председатель комиссии
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual async Task<List<AwpAssessingProgramSign>> CreateSigns(AwpAssessingProgram entity)
    {
        var signs = new List<AwpAssessingProgramSign>();
        var awpId = entity.Awp?.Id;
        var organizationId = entity.Organization?.Id;
        var committeeId = entity.Committee?.Id;
        if (!awpId.HasValue || !organizationId.HasValue || !committeeId.HasValue)
        {
            return signs;
        }

        var committee = await repository.GetById<AdmissionCommittee>(
            committeeId.Value,
            new EntityView(nameof(AdmissionCommittee.Views.AdmissionCommitteeE)));
        if (committee != null
            && IsCommitteeInScope(
                committee,
                awpId.Value,
                organizationId.Value,
                entity.Organization?.Hierarchy?.Id)
            && committee.Leader != null)
        {
            var sign = repository.MakeEntity<AwpAssessingProgramSign>();
            sign.Act = entity;
            sign.Employee = committee.Leader.Employee;
            sign.SignRole = await repository.GetById<SignRole>(ISignRole.Keys.CommissionChair);
            sign.Fio = committee.Leader.Employee.Fio;
            repository.Create(sign);
            // сначала ищем места работы по организации
            var poe = await repository.FirstOrDefault<PlaceOfEmployment>(
                x => x.Employee.Id == committee.Leader.Employee.Id
                     && x.Organization.Id == entity.Organization.Id);
            if (poe == null)
            {
                // затем ищем места работы по иерархии
                var MO = entity.Organization.Hierarchy == null
                    ? entity.Organization.Id
                    : entity.Organization.Hierarchy.Id;
                poe = await repository.FirstOrDefault<PlaceOfEmployment>(
                    x => x.Employee.Id == committee.Leader.Employee.Id
                        && (x.Organization.Id == MO || x.Organization.Hierarchy != null
                            && x.Organization.Hierarchy.Id == MO));
            }
            if (poe != null)
            {
                sign.PositionName = poe.Position.Name;
                sign.PartnerName = poe.Organization.Name;
            }
            signs.Add(sign);
        }
        return signs;
    }

    private static bool IsCommitteeInScope(
        AdmissionCommittee committee,
        Guid awpId,
        Guid organizationId,
        Guid? hierarchyId)
        => committee.Status == AdmissionCommitteeStatus.Approved
            && committee.Awp?.Id == awpId
            && (committee.Organization?.Id == organizationId
                || hierarchyId.HasValue && committee.Organization?.Id == hierarchyId.Value);
}

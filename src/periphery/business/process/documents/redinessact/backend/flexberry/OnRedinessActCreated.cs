namespace Tmobis.Business.Process.Documents;

using Common.Address;
using Common.Personnel;
using Tmobis.Business.Core.Flexberry.Models;
using Flexberry;
using Tmobis.Core.Entities;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;

public class OnRedinessActCreated(IRepository repository, EvaluationSheetSelectionProvider selectionProvider)
  : OnEntityUpdated<EntityCreated<RedinessAct>, RedinessAct>(null)
{
    protected override async ValueTask<List<IEntity>> ChangeEntity(RedinessAct entity)
    {
        var toUpdate = new List<IEntity>();
        toUpdate.AddRange(await CreateObjects(entity));
        toUpdate.AddRange(await CreateSigns(entity));
        return toUpdate;
    }

    protected virtual async Task<List<RedinessActObject>> CreateObjects(RedinessAct entity)
    {
        var objects = new List<RedinessActObject>();
        var wObjects = new List<WorkObject>();
        var organizationId = entity?.Organization?.Id;

        if (!organizationId.HasValue)
            return [];

        wObjects = repository.Query<WorkObject>()
            .Where(wobject => wobject.Actuality && wobject.Organization.Id == organizationId)
            .ToList();
        var evaluationSheets = await repository.Find<EvaluationSheet>(
            sheet => sheet.Awp.Id == entity.Awp.Id && sheet.Organization != null && sheet.Organization.Id == organizationId,
            view: EvaluationSheet.Views.EvaluationSheetPreparationEntityView);
        var selectedSheets = await selectionProvider.GetLatestByObject(evaluationSheets);
        int idx = 1;
        decimal avgIndex = 0;
        foreach (var workObject in wObjects)
        {
            var actObject = repository.MakeEntity<RedinessActObject>();
            actObject.WorkObject = workObject;
            actObject.Number = idx;
            actObject.Act = entity;

            var sheetObject = selectedSheets.GetValueOrDefault(workObject.Id);
            if (sheetObject != null)
            {
                actObject.ObjectReadiness = sheetObject.ObjectReadinessLevel;
                avgIndex += sheetObject.ObjectReadinessIndex;
            }
            objects.Add(actObject);
            repository.Create(actObject);
            idx++;
        }
        if (wObjects.Count > 0)
        {
            entity.AvgObjectReadinessIndex = avgIndex / (idx - 1);
            if (entity.AvgObjectReadinessIndex < 0.8m)
            {
                entity.PartnerReadiness = PartnerReadinessLevel.NotReady;
            }
            else if (entity.AvgObjectReadinessIndex >= 0.8m && entity.AvgObjectReadinessIndex < 0.9m)
            {
                entity.PartnerReadiness = PartnerReadinessLevel.ConditionReady;
            }
            else if (entity.AvgObjectReadinessIndex > 0.9m)
            {
                entity.PartnerReadiness = PartnerReadinessLevel.Ready;
            }
        }
        return objects;
    }

    /// <summary>
    /// При создании акта автоматически добавляются в подписанты: Председатель комиссии, Заместитель председателя комиссии и Члены комиссии
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual async Task<List<ReadinessActSign>> CreateSigns(RedinessAct entity)
    {
        var signs = new List<ReadinessActSign>();
        var status = AdmissionCommitteeStatus.Approved;
        var awpId = entity.Awp?.Id;
        var organizationId = entity.Organization?.Id;
        if (!awpId.HasValue || !organizationId.HasValue)
        {
            return signs;
        }

        AdmissionCommittee? committee;
        if (entity.AwpAssessingProgram != null)
        {
            var program = await repository.GetById<AwpAssessingProgram>(
                entity.AwpAssessingProgram.Id,
                new EntityView(nameof(AwpAssessingProgram.Views.AuditView)));
            if (program == null
                || program.Awp?.Id != awpId.Value
                || !IsOrganizationInScope(
                    program.Organization?.Id,
                    organizationId.Value,
                    entity.Organization?.Hierarchy?.Id)
                || program.Committee?.Id is not Guid committeeId)
            {
                return signs;
            }

            committee = await repository.GetById<AdmissionCommittee>(
                committeeId,
                new EntityView(nameof(AdmissionCommittee.Views.AdmissionCommitteeE)));
        }
        else
        {
            committee = await repository.FirstOrDefault<AdmissionCommittee>(
                x => x.Status == status
                    && x.Awp.Id == awpId.Value
                    && x.Organization.Id == organizationId.Value,
                new EntityView(nameof(AdmissionCommittee.Views.AdmissionCommitteeE)));
            if (committee == null && entity.Organization.Hierarchy != null)
            {
                committee = await repository.FirstOrDefault<AdmissionCommittee>(
                    x => x.Status == status
                        && x.Awp.Id == awpId.Value
                        && x.Organization.Id == entity.Organization.Hierarchy.Id,
                    new EntityView(nameof(AdmissionCommittee.Views.AdmissionCommitteeE)));
            }
        }

        if (committee != null
            && IsCommitteeInScope(
                committee,
                awpId.Value,
                organizationId.Value,
                entity.Organization?.Hierarchy?.Id))
        {
            if (committee.Leader != null)
            {
                signs.Add(await CreateSign(entity, committee.Leader.Employee, ISignRole.Keys.CommissionChair));
            }
            if (committee.DeputyLeader != null)
            {
                signs.Add(await CreateSign(entity, committee.DeputyLeader.Employee, ISignRole.Keys.CommissionDeputyChair));
            }
            // заполнить членами комиссии
            foreach (var comEmployee in committee.AdmissionCommitteeEmployee)
            {
                if ((committee.Leader == null || committee.Leader != null && committee.Leader.Id != comEmployee.Id)
                    && (committee.DeputyLeader == null || committee.DeputyLeader != null && committee.DeputyLeader.Id != comEmployee.Id))
                {
                    signs.Add(await CreateSign(entity, comEmployee.Employee, ISignRole.Keys.CommissionMember));
                }
            }
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
            && IsOrganizationInScope(committee.Organization?.Id, organizationId, hierarchyId);

    private static bool IsOrganizationInScope(
        Guid? candidateOrganizationId,
        Guid organizationId,
        Guid? hierarchyId)
        => candidateOrganizationId == organizationId
            || hierarchyId.HasValue && candidateOrganizationId == hierarchyId.Value;

    private async Task<ReadinessActSign> CreateSign(RedinessAct entity, Employee employee, Guid role)
    {
        var sign = repository.MakeEntity<ReadinessActSign>();
        sign.Act = entity;
        sign.Employee = employee;
        sign.SignRole = await repository.GetById<SignRole>(role);
        sign.Fio = employee.Fio;
        repository.Create(sign);
        // сначала ищем места работы по организации
        var poe = await repository.FirstOrDefault<PlaceOfEmployment>(x => x.Employee.Id == employee.Id && x.Organization.Id == entity.Organization.Id);
        if (poe == null)
        {
            // затем ищем места работы по иерархии
            var MO = entity.Organization.Hierarchy == null ? entity.Organization.Id : entity.Organization.Hierarchy.Id;
            poe = await repository.FirstOrDefault<PlaceOfEmployment>(x => x.Employee.Id == employee.Id
                && (x.Organization.Id == MO || x.Organization.Hierarchy != null && x.Organization.Hierarchy.Id == MO));
        }
        if (poe != null)
        {
            sign.PositionName = poe.Position.Name;
            sign.PartnerName = poe.Organization.Name;
        }
        return sign;
    }
}

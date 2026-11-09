extern alias verificationflexberry;
namespace Tmobis.Business.Process.Documents;

using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Entities.Consumers;
using Tmobis.Core.Entities.Messages;
using Tmobis.Core.Models;
using Tmobis.Core.Repositories;
using FlexVerificationRequierement = verificationflexberry::Tmobis.Business.Common.Resources.VerificationRequierement;

public class OnEvaluationSheetCreated(
    IRepository repository,
    EvaluationSheetService service)
  : OnEntityUpdated<EntityCreated<EvaluationSheet>, EvaluationSheet>(null)
{
    protected override async ValueTask PrepareEntity(EvaluationSheet entity)
    {
        // Установка значения по умолчанию для IsVerifiedTSO
        if (entity.Organization != null)
        {
            var organization = await repository.GetById<Organization>(entity.Organization.Id);
            if (organization?.OrganizationType != null)
            {
                // По умолчанию true для всех КА кроме типа "Прочие", false для типа "Прочие"
                entity.IsVerifiedTSO = !organization.OrganizationType.Id.Equals(OrganizationType.Keys.Other);
            }
            else
            {
                // Если тип организации не определен, устанавливаем true по умолчанию
                entity.IsVerifiedTSO = true;
            }
        }
        else
        {
            // Если организация не указана, устанавливаем true по умолчанию
            entity.IsVerifiedTSO = true;
        }
    }

    protected override async ValueTask<List<IEntity>> ChangeEntity(EvaluationSheet entity)
    {
        var toUpdate = new List<IEntity>();
        var details = await PopulateRequirements(entity);
        toUpdate.AddRange(details);
        toUpdate.AddRange(await CreateSigns(entity));
        return toUpdate;
    }

    private async Task<List<EvaluationSheetRequirement>> PopulateRequirements(EvaluationSheet document)
    {
        var objects = new List<EvaluationSheetRequirement>();
        if (document.Organization == null)
        {
            return objects;
        }

        var organization = await repository.GetById<Organization>(document.Organization.Id);
        if (organization?.OrganizationType == null)
        {
            return objects;
        }

        if (document.Requirements.Count > 0)
        {
            return objects;
        }

        var organizationTypeId = organization.OrganizationType.Id;
        var requirements = await repository.Find<FlexVerificationRequierement>(
            x => x.Actuality && x.OrganizationType.Id == organizationTypeId);
        if (requirements.Count == 0)
        {
            return objects;
        }
        var orderedRequirements = requirements
            .OrderBy(r => GetReqNumberSegments(r.Code), ReqNumberSegmentsComparer)
            .ThenBy(r => r.Code).ToList();

        if (!document.OwnerlessObjects && organizationTypeId == OrganizationType.Keys.Municipality)
        {
            orderedRequirements = orderedRequirements.Where(x => x.OwnerlessObjects).ToList();
        }

        if (document.OwnerlessObjects && organizationTypeId == OrganizationType.Keys.Municipality)
        {
            var empty = await repository.GetById<FlexVerificationRequierement>(FlexVerificationRequierement.Keys.Empty);
            var upperPart = orderedRequirements.Where(x => x.OwnerlessObjects).ToList();
            upperPart.Add(empty);
            var lowerPart = orderedRequirements.Where(x => !x.OwnerlessObjects && x.Code != "0").ToList();
            upperPart.AddRange(lowerPart);
            orderedRequirements = upperPart;
        }

        var fallbackOrder = 0;
        foreach (var requirement in orderedRequirements)
        {
            fallbackOrder++;
            var calculatedReq = GetCalculatedReq(requirement.Code, document);
            var detail = new EvaluationSheetRequirement
            {
                EvaluationSheet = document,
                VerificationRequirement = requirement,
                ShortReqName = requirement.ShortReqName,
                SupportingDocName = requirement.SupportingDocName,
                Weight = requirement.Weight,
                IsCalculated = requirement.IsCalculated,
                ReqNumber = requirement.ReqNumber,
                IndicatorName = requirement.IndicatorName,
                CalculatedReq = calculatedReq,
                ReqOrder = GetRequirementOrder(requirement) ?? fallbackOrder,
                SortOrder = fallbackOrder,
                IsOptional = requirement.IsOptional,
            };

            objects.Add(detail);
        }
        return objects;
    }

    private static int? GetRequirementOrder(IVerificationRequierement requirement)
    {
        if (int.TryParse(requirement.Code, out var orderFromCode))
        {
            return orderFromCode;
        }

        if (int.TryParse(requirement.ReqNumber, out var orderFromNumber))
        {
            return orderFromNumber;
        }

        return null;
    }

    private decimal GetCalculatedReq(string code, EvaluationSheet document)
    {
        decimal CalculatedReq = 0m;
        switch (code)
        {
            case "6":
                CalculatedReq = 1m;
                break;
            case "7":
                CalculatedReq = service.GetCountActs(document);
                break;
            case "8":
                CalculatedReq = service.GetCountOrganizations(document);
                break;
        }
        return CalculatedReq;
    }

    private static int[] GetReqNumberSegments(string reqNumber)
    {
        if (string.IsNullOrWhiteSpace(reqNumber))
        {
            return Array.Empty<int>();
        }

        var trimmed = reqNumber.Trim('.');
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return Array.Empty<int>();
        }

        return trimmed
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => int.TryParse(part, out var value) ? value : 0)
            .ToArray();
    }

    private static readonly IComparer<int[]> ReqNumberSegmentsComparer =
        new IntArrayComparer();

    private sealed class IntArrayComparer : IComparer<int[]>
    {
        public int Compare(int[]? x, int[]? y)
        {
            x ??= Array.Empty<int>();
            y ??= Array.Empty<int>();
            for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                var diff = x[i].CompareTo(y[i]);
                if (diff != 0)
                {
                    return diff;
                }
            }

            return x.Length.CompareTo(y.Length);
        }
    }

    /// <summary>
    /// При создании ОЛ автоматически добавляются в подписанты: Члены комиссии
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual async Task<List<EvaluationSheetSign>> CreateSigns(EvaluationSheet entity)
    {
        var signs = new List<EvaluationSheetSign>();
        if (entity.Awp == null || entity.Organization == null)
        {
            return signs;
        }

        var status = AdmissionCommitteeStatus.Approved;
        var awpId = entity.Awp.Id;
        var organizationId = entity.Organization.Id;
        var committee = await repository.FirstOrDefault<AdmissionCommittee>(x => x.Status == status
            && x.Awp.Id == awpId
            && x.Organization.Id == organizationId);
        if (committee == null)
        {
            if (entity.Organization.Hierarchy != null)
            {
                var hierarchyId = entity.Organization.Hierarchy.Id;
                committee = await repository.FirstOrDefault<AdmissionCommittee>(x => x.Status == status
                    && x.Awp.Id == awpId
                    && x.Organization.Id == hierarchyId);
            }
        }
        if (committee != null)
        {
            // заполнить членами комиссии
            foreach (var comEmployee in committee.AdmissionCommitteeEmployee)
            {
                if (committee.Leader != null && committee.Leader.Id != comEmployee.Id)
                {
                    signs.Add(await CreateSign(entity, comEmployee.Employee, ISignRole.Keys.CommissionMember));
                }
            }
        }
        return signs;
    }

    private async Task<EvaluationSheetSign> CreateSign(EvaluationSheet entity, Employee employee, Guid role)
    {
        var sign = repository.MakeEntity<EvaluationSheetSign>();
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

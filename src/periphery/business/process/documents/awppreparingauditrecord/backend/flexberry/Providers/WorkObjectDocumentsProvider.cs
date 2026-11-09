namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;

/// <summary>
/// Получение ссылок на документы с проверкой доступа ко всем объектам документа.
/// </summary>
public sealed class WorkObjectDocumentsProvider(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    RedinessActAccess readinessActAccess,
    AwpReadinessPassportAccess readinessPassportAccess,
    ICurrentRoleProvider roleProvider)
{
    public async Task<WorkObjectDocumentContext> GetContext(
        Guid objectId,
        Guid organizationId,
        Guid awpId,
        bool hasEvaluationSheet)
    {
        var documents = new List<WorkObjectDocument>();
        var roles = await roleProvider.GetCurrentRoleNames();
        var technical = await accessProvider.IsTechnicalAdministrator();
        var administration = technical || roles.Contains(WorkObjectRoleNames.Administration);
        var tso = roles.Contains(WorkObjectRoleNames.Tso);
        if (!administration && !tso && !roles.Contains("Член комиссии"))
        {
            return new(documents, new(false, false));
        }

        var actLinks = await repository.Find<RedinessActObject>(
            x => x.WorkObject.Id == objectId && x.Act.Awp.Id == awpId);
        var actIds = actLinks.Select(x => x.Act.Id).Distinct().ToList();
        var actIdValues = actIds.Cast<object>().ToList();
        var allActLinks = actIds.Count == 0
            ? []
            : await repository.Find<RedinessActObject>(x => actIdValues.Contains(x.Act.Id));
        var readableActIds = await GetReadableDocumentIds(
            allActLinks,
            x => x.Act.Id,
            x => x.WorkObject.Id);
        var acts = readableActIds.Count == 0
            ? []
            : await repository.GetByIds<RedinessAct>(
                readableActIds.ToList(),
                RedinessAct.Views.RedinessActLEntityView);
        foreach (var act in acts)
        {
            documents.Add(new(act.Id, "rediness-act", $"Акт {act.ActNumber}", act.ActStatus?.Name));
        }

        var signedReadinessAct = acts
            .Where(act => act.ActStatus?.Id == IActStatus.Keys.Signed)
            .OrderByDescending(act => act.ChangeStatusDate)
            .FirstOrDefault();
        var hasBlockingReadinessAct = acts.Any(act =>
            act.ActStatus?.Id != IActStatus.Keys.Signed
            || act.PartnerReadiness != PartnerReadinessLevel.NotReady);

        var passportLinks = await repository.Find<AwpReadinessPassportObject>(
            x => x.WorkObject.Id == objectId && x.Act.Awp.Id == awpId
                && (administration || x.Act.ActStatus.Id == IActStatus.Keys.Signed));
        var passportIds = passportLinks.Select(x => x.Act.Id).Distinct().ToList();
        var passportIdValues = passportIds.Cast<object>().ToList();
        var allPassportLinks = passportIds.Count == 0
            ? []
            : await repository.Find<AwpReadinessPassportObject>(
                x => passportIdValues.Contains(x.Act.Id));
        var readablePassportIds = await GetReadableDocumentIds(
            allPassportLinks,
            x => x.Act.Id,
            x => x.WorkObject.Id);
        var passports = readablePassportIds.Count == 0
            ? []
            : await repository.GetByIds<AwpReadinessPassport>(readablePassportIds.ToList());
        var hasReadinessPassport = false;
        foreach (var passport in passports)
        {
            documents.Add(new(passport.Id, "awp-readiness-passport",
                $"Паспорт {passport.ActNumber}", passport.ActStatus?.Name));
            hasReadinessPassport = true;
        }

        if (administration)
        {
            await AddPrograms(documents, objectId, awpId);
        }

        var canCreateAct = !hasBlockingReadinessAct
            && hasEvaluationSheet
            && await readinessActAccess.CanCreate(organizationId);
        var canCreatePassport = signedReadinessAct != null
            && !hasReadinessPassport
            && await readinessPassportAccess.CanCreate(organizationId, signedReadinessAct.Id);
        return new(documents, new(
            canCreateAct,
            canCreatePassport));
    }

    private async Task AddPrograms(List<WorkObjectDocument> documents, Guid objectId, Guid awpId)
    {
        var programLinks = await repository.Find<AwpAssessingProgramObject>(
            x => x.WorkObject.Id == objectId && x.Program.Awp.Id == awpId);
        var programIds = programLinks.Select(x => x.Program.Id).Distinct().ToList();
        var programIdValues = programIds.Cast<object>().ToList();
        var allProgramLinks = programIds.Count == 0
            ? []
            : await repository.Find<AwpAssessingProgramObject>(
                x => programIdValues.Contains(x.Program.Id));
        var readableProgramIds = await GetReadableDocumentIds(
            allProgramLinks,
            x => x.Program.Id,
            x => x.WorkObject.Id);
        var programs = readableProgramIds.Count == 0
            ? []
            : await repository.GetByIds<AwpAssessingProgram>(readableProgramIds.ToList());
        foreach (var program in programs)
        {
            documents.Add(new(program.Id, "awp-assessing-program",
                $"Программа оценки {program.ActNumber}", program.ActStatus?.Name));
            if (program.Committee != null)
            {
                documents.Add(new(program.Committee.Id, "admission-committee", "Комиссия", null));
            }
        }
    }

    private async Task<HashSet<Guid>> GetReadableDocumentIds<TLink>(
        IEnumerable<TLink> links,
        Func<TLink, Guid> getDocumentId,
        Func<TLink, Guid> getObjectId)
    {
        var groups = links.GroupBy(getDocumentId).ToList();
        var objectIds = groups
            .SelectMany(group => group.Select(getObjectId))
            .Distinct()
            .ToList();
        var allowed = await accessProvider.GetAccessibleWorkObjectIds(objectIds);
        return groups
            .Where(group => group.Any() && group.Select(getObjectId).All(allowed.Contains))
            .Select(group => group.Key)
            .ToHashSet();
    }
}

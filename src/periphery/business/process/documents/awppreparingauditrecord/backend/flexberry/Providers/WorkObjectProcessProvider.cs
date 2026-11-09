namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Process.Documents.Models;
using Tmobis.Core.Repositories;

public sealed class WorkObjectProcessProvider(
    IRepository repository,
    IWorkObjectAccessProvider accessProvider,
    IAwpPreparingAuditRecordProvider preparationProvider,
    WorkObjectDocumentsProvider documentsProvider,
    WorkObjectRequirementMapper requirementMapper,
    EvaluationSheetReviewService reviewService) : IWorkObjectProcessProvider
{
    public async Task<WorkObjectPreparationInfo> GetPreparation(Guid workObjectId, Guid? awpId)
    {
        var awp = await GetAwp(awpId);
        await EnsureCanAccess(workObjectId, awp?.Id);
        return (await preparationProvider.GetObjectPreparationInfo(awp?.Id, [workObjectId])).Single();
    }

    public async Task<WorkObjectProcess> GetProcess(Guid workObjectId, Guid? awpId)
    {
        var awp = await GetAwp(awpId);
        var workObject = await repository.GetById<WorkObject>(workObjectId,
            WorkObject.Views.WorkObjectEEntityView)
            ?? throw new ArgumentException("Объект не найден.");
        var organization = workObject.Organization
            ?? throw new ArgumentException("У объекта не указана организация.");
        var permissions = await EnsureCanAccess(workObjectId, awp?.Id, workObject);

        var preparation = (await preparationProvider.GetObjectPreparationInfo(
            awp?.Id, [workObjectId])).Single();
        var requirements = preparation.EvaluationSheetId.HasValue
            ? await repository.Find<EvaluationSheetRequirement>(
                x => x.EvaluationSheet.Id == preparation.EvaluationSheetId.Value,
                view: EvaluationSheetRequirement.Views.EvaluationSheetRequirementEEntityView)
            : [];
        var tsoLinks = await repository.Find<WorkObjectTsoOrganization>(
            x => x.WorkObject.Id == workObjectId);
        var tsoIds = tsoLinks.Select(x => x.Organization.Id).Distinct().ToList();
        var tsos = await repository.Find<Organization>(x => tsoIds.Contains(x.Id),
            view: Organization.Views.OrganizationEEntityView);
        if (organization.Hierarchy != null)
        {
            await repository.SubLoad(organization.Hierarchy,
                Organization.Views.OrganizationEEntityView);
        }

        var documentContext = awp == null
            ? new WorkObjectDocumentContext([], new(false, false))
            : await documentsProvider.GetContext(
                workObjectId,
                organization.Id,
                awp.Id,
                permissions.CanFormAct);
        return new WorkObjectProcess(workObjectId, awp?.Id, awp?.Name,
            new WorkspaceOption(organization.Id, organization.Name),
            organization.Hierarchy == null ? null
                : new WorkspaceOption(organization.Hierarchy.Id, organization.Hierarchy.Name),
            tsos.Select(x => new WorkspaceOption(x.Id, x.Name)).ToList(),
            preparation,
            await requirementMapper.Map(requirements.OrderBy(x => x.SortOrder)),
            documentContext.Documents,
            documentContext.Actions,
            permissions.CanSubmit && tsos.Count > 0
                && awp != null && preparation.CanSubmitForReview,
            permissions.CanReview,
            preparation.ReadinessLevel switch
            {
                LevelReadiness.Ready => "Готов",
                LevelReadiness.ReadyWithConditions => "Готов с условиями",
                _ => "Не готов"
            })
        {
            CanFormAct = permissions.CanFormAct
        };
    }

    private async Task<EvaluationSheetPermissions> EnsureCanAccess(
        Guid workObjectId, Guid? awpId, WorkObject? workObject = null)
    {
        var permissions = await reviewService.GetPermissions(workObjectId, awpId, workObject);
        if (!permissions.CanReview && !permissions.CanSubmit)
        {
            await accessProvider.EnsureCanAccess(workObjectId);
        }
        return permissions;
    }

    private async Task<Awp?> GetAwp(Guid? awpId)
    {
        var awps = await repository.Find<Awp>(x => x.Actuality,
            view: Awp.Views.AwpEEntityView);
        var awp = awpId.HasValue
            ? awps.FirstOrDefault(x => x.Id == awpId.Value)
            : awps.OrderByDescending(x => x.IsCurrent).ThenByDescending(x => x.DateBegin).FirstOrDefault();
        if (awpId.HasValue && awp == null)
        {
            throw new ArgumentException("Выбранный ОЗП не найден или неактуален.");
        }
        return awp;
    }

}

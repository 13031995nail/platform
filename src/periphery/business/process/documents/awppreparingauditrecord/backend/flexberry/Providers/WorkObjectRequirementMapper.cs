namespace Tmobis.Business.Process.Documents.Providers;

using Tmobis.Business.Process.Documents.Models;
using Tmobis.Core.Entities;
using Tmobis.Core.Repositories;
using StoredFile = Tmobis.Business.Core.Flexberry.Models.File;

public sealed class WorkObjectRequirementMapper(IRepository repository)
{
    public async Task<IReadOnlyList<WorkObjectRequirement>> Map(IEnumerable<EvaluationSheetRequirement> requirements)
    {
        var items = requirements.ToList();
        var ids = items.SelectMany(x => GetFileIds(x.SupportingDocFile)).Distinct().ToList();
        var files = ids.Count == 0 ? [] : await repository.Find<StoredFile>(x => ids.Contains(x.Id),
            view: StoredFile.Views.FileInfoEntityView);
        var attachments = files.ToDictionary(x => x.Id,
            x => new WorkObjectAttachment(x.Id, x.Name, x.Creator, x.CreateTime));
        return items.Select(requirement => Map(requirement,
            GetFileIds(requirement.SupportingDocFile).Where(attachments.ContainsKey)
                .Select(id => attachments[id]).ToList())).ToList();
    }

    private static IEnumerable<Guid> GetFileIds(string? value)
        => (value ?? string.Empty).Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => Guid.TryParse(value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty).Distinct();

    private static WorkObjectRequirement Map(EvaluationSheetRequirement requirement,
        IReadOnlyList<WorkObjectAttachment> attachments)
        => new(requirement.Id, requirement.ReqNumber, requirement.ShortReqName,
            requirement.SupportingDocName, requirement.SupportingDocFile,
            EvaluationSheetProgress.IsCounted(requirement),
            EvaluationSheetProgress.IsFilled(requirement),
            EvaluationSheetProgress.HasMissingDocument(requirement),
            requirement.ProblemComment, requirement.Deadline,
            requirement.IsCompletedReq, requirement.IsVerified, attachments);
}

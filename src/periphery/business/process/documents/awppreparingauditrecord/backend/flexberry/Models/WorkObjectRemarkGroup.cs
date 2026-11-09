namespace Tmobis.Business.Process.Documents.Models;

public sealed record WorkObjectRemarkGroup(
    Guid WorkObjectId, string? Caption, Guid EvaluationSheetId,
    IReadOnlyList<WorkObjectRequirement> Requirements);

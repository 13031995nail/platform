namespace Tmobis.Business.Process.Documents.Models;

using Tmobis.Business.Process.Documents.Providers;

public sealed record WorkObjectProcess(
    Guid WorkObjectId,
    Guid? AwpId,
    string? AwpCaption,
    WorkspaceOption Organization,
    WorkspaceOption? Territory,
    IReadOnlyList<WorkspaceOption> TsoOrganizations,
    WorkObjectPreparationInfo Preparation,
    IReadOnlyList<WorkObjectRequirement> Requirements,
    IReadOnlyList<WorkObjectDocument> Documents,
    WorkObjectDocumentActions DocumentActions,
    bool CanSubmit,
    bool CanReview,
    string ReadinessCaption)
{
    public bool CanFormAct { get; init; }
}

public sealed record WorkObjectRequirement(
    Guid Id,
    string? Number,
    string? Name,
    string? DocumentName,
    string? DocumentFile,
    bool IsCounted,
    bool IsFilled,
    bool HasMissingDocument,
    string? Remark,
    DateTime? Deadline,
    bool IsCompleted,
    bool IsVerified,
    IReadOnlyList<WorkObjectAttachment> Attachments);

public sealed record WorkObjectAttachment(Guid Id, string Name, string? Creator, DateTime? CreatedAt);

public sealed record WorkObjectDocument(Guid Id, string Route, string Caption, string? Status);

public sealed record WorkObjectDocumentActions(
    bool CanCreateReadinessAct,
    bool CanCreateReadinessPassport);

public sealed record WorkObjectDocumentContext(
    IReadOnlyList<WorkObjectDocument> Documents,
    WorkObjectDocumentActions Actions);

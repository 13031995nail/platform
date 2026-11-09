namespace Tmobis.Business.Process.Documents.Models;

/// <summary>
/// Параметры ролевой сводки.
/// </summary>
public sealed class WorkspaceDashboardQuery
{
    public string? Workspace { get; set; }
    public Guid? AwpId { get; set; }
    public Guid? TerritoryId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? OrganizationTypeId { get; set; }
    public Guid? WorkObjectId { get; set; }
    public string? State { get; set; }
    public string? ReadinessLevel { get; set; }
    public string? Search { get; set; }
}

/// <summary>
/// Данные стартовой страницы рабочего пространства.
/// </summary>
public sealed record WorkspaceDashboard(
    string Workspace,
    Guid? AwpId,
    string? AwpCaption,
    WorkspaceStatistics Statistics,
    WorkspaceFilterOptions Filters,
    IReadOnlyList<WorkspaceObject> Objects);

/// <summary>
/// Независимые счётчики рабочего пространства.
/// </summary>
public sealed record WorkspaceStatistics(
    int Total,
    int NotStarted,
    int InProgress,
    int UnderReview,
    int Remarks,
    int Ready,
    int Completed,
    int ReadyObjects,
    int ReadyWithConditions,
    int NotReady,
    int MissingDocuments,
    int OverdueRemarks,
    int RequiresAction);

/// <summary>
/// Допустимые значения фильтров дашборда.
/// </summary>
public sealed record WorkspaceFilterOptions(
    IReadOnlyList<WorkspaceOption> Awps,
    IReadOnlyList<WorkspaceOption> Territories,
    IReadOnlyList<WorkspaceOption> Organizations,
    IReadOnlyList<WorkspaceOption> OrganizationTypes,
    IReadOnlyList<WorkspaceOption> WorkObjects,
    IReadOnlyList<WorkspaceTextOption> States,
    IReadOnlyList<WorkspaceTextOption> ReadinessLevels);

public sealed record WorkspaceOption(Guid Id, string? Caption);

public sealed record WorkspaceTextOption(string Id, string Caption);

public sealed record WorkspaceObject(
    Guid Id,
    string? Caption,
    string State,
    string ReadinessLevel,
    bool HasMissingDocuments,
    bool HasOverdueRemarks,
    decimal? CompletionPercent,
    string? Organization,
    string? Address);

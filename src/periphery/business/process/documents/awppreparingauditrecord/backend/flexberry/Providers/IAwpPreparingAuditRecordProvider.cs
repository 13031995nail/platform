namespace Tmobis.Business.Process.Documents.Providers;

/// <summary>
/// Методы ЖК.
/// </summary>
public interface IAwpPreparingAuditRecordProvider
{
    /// <summary>
    /// Вычисление индекса по организации
    /// </summary>
    public Task UpdatePartnerRecords(Guid awpId, Guid organizationId);

    /// <summary>
    /// Получить фактическое состояние подготовки объектов в рамках ОЗП.
    /// Данные формируются напрямую из оценочных листов, актов и паспортов,
    /// поэтому наличие записей журнала контроля не является обязательным.
    /// </summary>
    public Task<IReadOnlyList<WorkObjectPreparationInfo>> GetObjectPreparationInfo(
        Guid? awpId,
        IReadOnlyCollection<Guid> workObjectIds);
}

/// <summary>
/// Состояния подготовки объекта к прохождению ОЗП.
/// </summary>
public static class WorkObjectPreparationStates
{
    public const string NotStarted = "notStarted";
    public const string InProgress = "inProgress";
    public const string UnderReview = "underReview";
    public const string Remarks = "remarks";
    public const string Ready = "ready";
    public const string Completed = "completed";
}

/// <summary>
/// Фактические показатели подготовки объекта в рамках ОЗП.
/// </summary>
public sealed record WorkObjectPreparationInfo(
    Guid WorkObjectId,
    string State,
    LevelReadiness ReadinessLevel,
    bool HasMissingDocuments,
    bool HasOverdueRemarks,
    RequirementProgress? Progress = null,
    Guid? EvaluationSheetId = null,
    decimal? ReadinessIndex = null)
{
    public bool CanChangeEvaluationSheet { get; init; }
    public Guid? DraftEvaluationSheetId { get; init; }
    public bool CanSubmitForReview { get; init; }
    public bool CanStartReview { get; init; }
    public bool CanAccept { get; init; }
    public DateTime? ReviewRequestedAt { get; init; }
    public DateTime? ReviewStartedAt { get; init; }
}

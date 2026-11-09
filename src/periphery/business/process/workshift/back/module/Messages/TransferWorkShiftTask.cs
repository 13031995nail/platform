namespace Tmobis.Business.Process.WorkShift.Messages;

using Tmobis.Core.Medium.Models;

/// <summary>
/// Перенос пункта смены на будущую дату.
/// </summary>
[MessageMetadata(Group = "WorkShift")]
public record TransferWorkShiftTask : DomainCommand<TransferWorkShiftTaskResult>
{
    /// <summary>
    /// Идентификатор переносимого пункта смены.
    /// </summary>
    public required Guid WorkShiftTaskId { get; init; }

    /// <summary>
    /// Дата целевой смены.
    /// </summary>
    public required DateOnly TargetDate { get; init; }
}

/// <summary>
/// Результат переноса пункта смены.
/// </summary>
public record TransferWorkShiftTaskResult : StatusMessageResult
{
    /// <summary>
    /// Текст ошибки или пояснения.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Идентификатор созданного пункта смены.
    /// </summary>
    public Guid? NewWorkShiftTaskId { get; init; }

    /// <summary>
    /// Идентификатор смены на целевую дату.
    /// </summary>
    public Guid? TargetWorkShiftId { get; init; }
}

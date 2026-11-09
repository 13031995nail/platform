namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Models;

public class WorkShiftTask : BusinessEntity, IWorkShiftTask
{
    public virtual Guid WorkShiftId { get; set; }

    public virtual IWorkShift WorkShift { get; set; } = null!;

    public virtual DateTime? PlannedStartTime { get; set; }

    public virtual DateTime? ActualStartTime { get; set; }

    public virtual DateTime? ActualEndTime { get; set; }

    /// <inheritdoc/>
    public virtual double? ActualStartLatitude { get; set; }

    /// <inheritdoc/>
    public virtual double? ActualStartLongitude { get; set; }

    /// <inheritdoc/>
    public virtual double? ActualEndLatitude { get; set; }

    /// <inheritdoc/>
    public virtual double? ActualEndLongitude { get; set; }

    public virtual Guid WorkShiftStatusId { get; set; }

    public virtual IWorkShiftStatus WorkShiftStatus { get; set; } = null!;

    public virtual string? Note { get; set; }

    /// <inheritdoc/>
    public virtual Guid? PreviousWorkShiftTaskId { get; set; }

    /// <inheritdoc/>
    public virtual IWorkShiftTask? PreviousWorkShiftTask { get; set; }
}

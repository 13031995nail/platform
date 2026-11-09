namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Medium.Models;

[MessageMetadata(Group = "WorkShift")]
public record FinishWorkShiftTask : MessageWorkShiftTask
{
}

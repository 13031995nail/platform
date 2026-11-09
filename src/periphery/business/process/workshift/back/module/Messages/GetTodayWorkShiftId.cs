namespace Tmobis.Business.Process.WorkShift.Messages;

using Tmobis.Core.Medium.Models;

[MessageMetadata("WorkShift", "GetTodayWorkShiftId")]
public record GetTodayWorkShiftId : DomainQuery<GetTodayWorkShiftIdResult>;

public record GetTodayWorkShiftIdResult : IMessageResult
{
    public Guid? WorkShiftId { get; init; }
}

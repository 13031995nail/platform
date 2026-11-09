
namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Medium.Models;

public record MessageWorkShiftTask : DomainCommand<StatusMessageResult>
{
    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public DateTime Time { get; set; }

    public required Guid WorkShiftTaskId { get; init; }
}


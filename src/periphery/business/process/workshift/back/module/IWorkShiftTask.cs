namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public interface IWorkShiftTask : IRegisteredEntity, IWWorkShiftStatus
{
    Guid WorkShiftId { get; set; }

    IWorkShift WorkShift { get; set; }

    DateTime? PlannedStartTime { get; set; }

    DateTime? ActualStartTime { get; set; }

    DateTime? ActualEndTime { get; set; }

    /// <summary>
    /// Широта фактической геометки начала.
    /// </summary>
    /// <remarks>
    /// Альтернатива: NetTopologySuite.Geometries.Point + PostGIS
    /// (пакет Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite)
    /// для нативного типа geometry(Point, 4326) с пространственными индексами.
    /// </remarks>
    double? ActualStartLatitude { get; set; }

    /// <summary>
    /// Долгота фактической геометки начала.
    /// </summary>
    double? ActualStartLongitude { get; set; }

    /// <summary>
    /// Широта фактической геометки окончания.
    /// </summary>
    double? ActualEndLatitude { get; set; }

    /// <summary>
    /// Долгота фактической геометки окончания.
    /// </summary>
    double? ActualEndLongitude { get; set; }

    string? Note { get; set; }

    /// <summary>
    /// Предыдущий пункт смены, с которого выполнен перенос.
    /// </summary>
    Guid? PreviousWorkShiftTaskId { get; set; }

    /// <summary>
    /// Предыдущий пункт смены, с которого выполнен перенос.
    /// </summary>
    IWorkShiftTask? PreviousWorkShiftTask { get; set; }

    public class Views
    {
        public static readonly EntityView WorkShiftTaskL
            = new(
                nameof(WorkShiftTaskL),
                [
                    nameof(WorkShiftStatus),
                ]);

        public static readonly EntityView WorkShiftTaskE
            = new(
                nameof(WorkShiftTaskE),
                [
                    nameof(WorkShift),
                    $"{nameof(WorkShift)}.{nameof(WorkShiftStatus)}",
                    nameof(WorkShiftStatus),
                    nameof(PreviousWorkShiftTask),
                ]);
    }
}

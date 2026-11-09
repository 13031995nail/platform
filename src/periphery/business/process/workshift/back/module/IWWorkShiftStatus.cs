namespace Tmobis.Business.Process.WorkShift;

public interface IWWorkShiftStatus
{
    Guid WorkShiftStatusId { get; set; }

    IWorkShiftStatus WorkShiftStatus { get; set; }
}

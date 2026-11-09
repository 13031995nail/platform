namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Models;

public class WorkShift : BusinessEntity, IWorkShift
{
    public virtual DateOnly Date { get; set; }

    public virtual Guid EmployeeId { get; set; }

    public virtual IEmployee Employee { get; set; } = null!;

    public virtual Guid WorkShiftStatusId { get; set; }

    public virtual IWorkShiftStatus WorkShiftStatus { get; set; } = null!;

    public virtual List<IWorkShiftTask> Tasks { get; set; } = [];
}

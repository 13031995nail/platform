namespace Tmobis.Business.Process.WorkShift;

using Tmobis.Business.Core.Models.Employees;
using Tmobis.Core.Entities;
using Tmobis.Core.Models;

public interface IWorkShift : IRegisteredEntity, IWEmployee, IWWorkShiftStatus
{
    DateOnly Date { get; set; }

    List<IWorkShiftTask> Tasks { get; set; }

    public class Views
    {
        public static readonly EntityView WorkShiftL
            = new(
                nameof(WorkShiftL),
                [
                    nameof(Employee),
                    nameof(WorkShiftStatus),
                ]);

        public static readonly EntityView WorkShiftE
            = new(
                nameof(WorkShiftE),
                [
                    nameof(Employee),
                    nameof(WorkShiftStatus),
                    nameof(Tasks),
                ],
                searchProperties: [
                    nameof(Date),
                ]);
    }
}

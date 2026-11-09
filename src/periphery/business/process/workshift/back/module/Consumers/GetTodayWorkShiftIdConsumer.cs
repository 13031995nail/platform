namespace Tmobis.Business.Process.WorkShift.Consumers;

using Tmobis.Business.Core.Providers.Users;
using Tmobis.Business.Process.WorkShift.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;

public sealed class GetTodayWorkShiftIdConsumer(
    IRepository repository,
    ICurrentUserProvider currentUserProvider)
: IMessageConsumer<GetTodayWorkShiftId, GetTodayWorkShiftIdResult>
{
    public async Task<GetTodayWorkShiftIdResult> ConsumeWithResult(GetTodayWorkShiftId message)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var employee = await currentUserProvider.GetCurrentEmployee();

        IWorkShift? workShift = null;
        if (employee != null)
        {
            workShift = await repository.FirstOrDefault<IWorkShift>(
                shift => shift.Date == today && shift.EmployeeId == employee.Id,
                IWorkShift.Views.WorkShiftL);
        }

        return new GetTodayWorkShiftIdResult
        {
            WorkShiftId = workShift?.Id,
        };
    }
}

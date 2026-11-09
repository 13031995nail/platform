namespace Tmobis.Business.Process.WorkShift.Consumers;

using System.Threading.Tasks;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Repositories;

public class FinishWorkShiftTaskConsumer(
    IRepository repository
) : IMessageConsumer<FinishWorkShiftTask, StatusMessageResult>
{
    /// <inheritdoc/>
    public async Task<StatusMessageResult> ConsumeWithResult(FinishWorkShiftTask msg)
    {
        try
        {
            var workShiftTask = await repository.GetById<IWorkShiftTask>(msg.WorkShiftTaskId)
                ?? throw new InvalidOperationException("Не найден пункт смены");
            workShiftTask.ActualEndLatitude = msg.Latitude;
            workShiftTask.ActualEndLongitude = msg.Longitude;
            workShiftTask.ActualEndTime = msg.Time;
            workShiftTask.WorkShiftStatusId = IWorkShiftStatus.Keys.Completed;
            return new StatusMessageResult { Error = false };
        }
        catch (Exception ex)
        {
            return new StatusMessageResult { Error = true };
        }
    }
}

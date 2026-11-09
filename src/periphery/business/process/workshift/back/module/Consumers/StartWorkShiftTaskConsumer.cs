namespace Tmobis.Business.Process.WorkShift.Consumers;

using System.Threading.Tasks;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Models;
using Tmobis.Core.Repositories;

public class StartWorkShiftTaskConsumer(
    IRepository repository
) : IMessageConsumer<StartWorkShiftTask, StatusMessageResult>
{
    /// <inheritdoc/>
    public async Task<StatusMessageResult> ConsumeWithResult(StartWorkShiftTask msg)
    {
        try
        {
            var workShiftTask = await repository.GetById<IWorkShiftTask>(msg.WorkShiftTaskId)
                ?? throw new InvalidOperationException("Не найден пункт смены");
            workShiftTask.ActualStartLatitude = msg.Latitude;
            workShiftTask.ActualStartLongitude = msg.Longitude;
            workShiftTask.ActualStartTime = msg.Time;
            workShiftTask.WorkShiftStatusId = IWorkShiftStatus.Keys.InProgress;
            return new StatusMessageResult { Error = false };
        }
        catch (Exception ex)
        {
            return new StatusMessageResult { Error = true };
        }
    }
}

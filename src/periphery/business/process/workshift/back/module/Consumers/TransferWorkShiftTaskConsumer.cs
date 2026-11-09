namespace Tmobis.Business.Process.WorkShift.Consumers;

using Tmobis.Business.Process.WorkShift.Messages;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Repositories;

/// <summary>
/// Перенос пункта смены на будущую дату: статус «Перенесено»,
/// создание нового пункта со ссылкой на предыдущий и get-or-create смены.
/// </summary>
public sealed class TransferWorkShiftTaskConsumer(
    IRepository repository
) : IMessageConsumer<TransferWorkShiftTask, TransferWorkShiftTaskResult>
{
    private static readonly Guid[] NonTransferableStatuses =
    [
        IWorkShiftStatus.Keys.Postponed,
        IWorkShiftStatus.Keys.Completed,
    ];

    /// <inheritdoc/>
    public async Task<TransferWorkShiftTaskResult> ConsumeWithResult(TransferWorkShiftTask msg)
    {
        try
        {
            var sourceTask = await repository.GetById<IWorkShiftTask>(
                    msg.WorkShiftTaskId,
                    IWorkShiftTask.Views.WorkShiftTaskE)
                ?? throw new InvalidOperationException("Не найден пункт смены.");

            var sourceShift = sourceTask.WorkShift
                ?? throw new InvalidOperationException("Не найдена смена пункта.");

            ValidateTransfer(sourceTask, sourceShift, msg.TargetDate);

            var targetShift = await GetOrCreateWorkShift(
                sourceShift.EmployeeId,
                msg.TargetDate);

            sourceTask.WorkShiftStatusId = IWorkShiftStatus.Keys.Postponed;

            var newTask = repository.MakeEntity<IWorkShiftTask>();
            newTask.WorkShiftId = targetShift.Id;
            newTask.WorkShiftStatusId = IWorkShiftStatus.Keys.NotApproved;
            newTask.Note = sourceTask.Note;
            newTask.PreviousWorkShiftTaskId = sourceTask.Id;
            newTask.PlannedStartTime = MapPlannedStartTime(
                sourceTask.PlannedStartTime,
                msg.TargetDate);

            repository.Create(newTask);

            return new TransferWorkShiftTaskResult
            {
                Error = false,
                NewWorkShiftTaskId = newTask.Id,
                TargetWorkShiftId = targetShift.Id,
            };
        }
        catch (InvalidOperationException ex)
        {
            return new TransferWorkShiftTaskResult
            {
                Error = true,
                Message = ex.Message,
            };
        }
        catch (Exception)
        {
            return new TransferWorkShiftTaskResult
            {
                Error = true,
                Message = "Ошибка при переносе пункта смены.",
            };
        }
    }

    private static void ValidateTransfer(
        IWorkShiftTask sourceTask,
        IWorkShift sourceShift,
        DateOnly targetDate)
    {
        if (NonTransferableStatuses.Contains(sourceTask.WorkShiftStatusId))
        {
            throw new InvalidOperationException(
                "Нельзя перенести пункт смены в статусе «Перенесено» или «Выполнено».");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (targetDate <= today)
        {
            throw new InvalidOperationException(
                "Дата переноса должна быть больше текущей даты.");
        }

        if (targetDate <= sourceShift.Date)
        {
            throw new InvalidOperationException(
                "Дата переноса должна быть больше даты текущей смены.");
        }
    }

    private async Task<IWorkShift> GetOrCreateWorkShift(Guid employeeId, DateOnly date)
    {
        var existing = await repository.FirstOrDefault<IWorkShift>(
            shift => shift.Date == date && shift.EmployeeId == employeeId,
            IWorkShift.Views.WorkShiftL);

        if (existing != null)
        {
            return existing;
        }

        var created = repository.MakeEntity<IWorkShift>();
        created.Date = date;
        created.EmployeeId = employeeId;
        created.WorkShiftStatusId = IWorkShiftStatus.Keys.NotApproved;
        repository.Create(created);
        return created;
    }

    private static DateTime? MapPlannedStartTime(DateTime? plannedStartTime, DateOnly targetDate)
    {
        if (plannedStartTime is null)
        {
            return null;
        }

        var planned = plannedStartTime.Value;
        return new DateTime(
            targetDate.Year,
            targetDate.Month,
            targetDate.Day,
            planned.Hour,
            planned.Minute,
            planned.Second,
            planned.Kind);
    }
}

import { WorkShiftStatusKeys } from '../constants';
import type { WorkShiftTask } from '../models/work-shift-task';

/**
 * Проверка возможности переноса пункта смены на будущую дату.
 */
export function canTransferWorkShiftTask(task?: WorkShiftTask | null): boolean {
  if (!task?.id) {
    return false;
  }

  const statusId = task.workShiftStatusId ?? task.workShiftStatus?.id;
  if (!statusId) {
    return true;
  }

  return (
    statusId !== WorkShiftStatusKeys.Postponed
    && statusId !== WorkShiftStatusKeys.Completed
  );
}

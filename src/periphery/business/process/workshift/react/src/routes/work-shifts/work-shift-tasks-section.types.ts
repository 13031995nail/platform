import type useEntityList from '@tmobis/core/components/entity-list/use-entity-list';
import type { WorkShiftTask } from '../../models/work-shift-task';

export type TasksListData = ReturnType<typeof useEntityList<WorkShiftTask>>;

export type WorkShiftTasksSectionProps = {
  listData: TasksListData;
  onCreateTask: () => void;
  onTaskRowClick: (task?: WorkShiftTask) => void;
  /** На вкладке заголовок секции не нужен — его даёт tab strip. */
  showTitle?: boolean;
  /** Дата текущей смены (YYYY-MM-DD) для валидации переноса. */
  workShiftDate?: string | null;
};

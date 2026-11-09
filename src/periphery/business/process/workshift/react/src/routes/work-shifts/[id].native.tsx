import EntityEdit, { EditFormProps } from '@tmobis/core/components/entity-edit';
import { defaultSlots, FormSlot } from '@tmobis/core/components/entity-edit/field-extension';
import { useWorkShiftEdit } from './use-work-shift-edit';
import { WorkShiftFields } from './work-shift-fields';
import { WorkShiftTasksSection } from './work-shift-tasks-section';

/**
 * Нативная форма смены: «Общее» и «Пункты смены» — отдельные вкладки.
 * Metro сам выбирает этот файл вместо [id].tsx на iOS/Android.
 */
export default function WorkShiftEditFormNative({ isNew }: EditFormProps) {
  const {
    data,
    employeeLookup,
    tasksListData,
    workShiftStatusName,
    workShiftDate,
    onTaskRowClick,
    onCreateTask,
  } = useWorkShiftEdit(isNew);

  return (
    <EntityEdit
      {...data}
      flattenTabs={false}
      tabTitleMap={{
        tasks: 'Пункты смены',
      }}
    >
      <FormSlot slot={defaultSlots.leftTop}>
        <WorkShiftFields
          workShiftStatusName={workShiftStatusName}
          employeeLookup={employeeLookup}
        />
      </FormSlot>
      {!isNew ? (
        <FormSlot slot={defaultSlots.leftTop} tab="tasks">
          <WorkShiftTasksSection
            listData={tasksListData}
            onCreateTask={onCreateTask}
            onTaskRowClick={onTaskRowClick}
            showTitle={false}
            workShiftDate={workShiftDate}
          />
        </FormSlot>
      ) : null}
    </EntityEdit>
  );
}

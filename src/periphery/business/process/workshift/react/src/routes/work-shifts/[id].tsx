import EntityEdit, { EditFormProps } from '@tmobis/core/components/entity-edit';
import { defaultSlots, FormSlot } from '@tmobis/core/components/entity-edit/field-extension';
import { useWorkShiftEdit } from './use-work-shift-edit';
import { WorkShiftFields } from './work-shift-fields';
import { WorkShiftTasksSection } from './work-shift-tasks-section';

/**
 * Веб-форма смены: поля слева, пункты смены справа (одна страница).
 * Натив подхватывает соседний [id].native.tsx через Metro.
 */
export default function WorkShiftEditForm({ isNew }: EditFormProps) {
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
        <EntityEdit {...data}>
            <FormSlot slot={defaultSlots.leftTop}>
                <WorkShiftFields
                    workShiftStatusName={workShiftStatusName}
                    employeeLookup={employeeLookup}
                />
            </FormSlot>
            {!isNew ? (
                <FormSlot slot={defaultSlots.rightTop}>
                    <WorkShiftTasksSection
                        listData={tasksListData}
                        onCreateTask={onCreateTask}
                        onTaskRowClick={onTaskRowClick}
                        workShiftDate={workShiftDate}
                    />
                </FormSlot>
            ) : null}
        </EntityEdit>
    );
}

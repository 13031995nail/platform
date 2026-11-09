import { useCallback, useMemo, useState } from 'react';
import EntityList from '@tmobis/core/components/entity-list';
import { Button, ButtonText } from '@tmobis/core/components/ui/button';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { CalendarDaysIcon } from '@tmobis/core/components/ui/icon';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { TransferWorkShiftTaskModal } from '../../components/transfer-work-shift-task-modal';
import { formatPlannedStartTime } from '../../utils/planned-start-time';
import { canTransferWorkShiftTask } from '../../utils/work-shift-task-transfer';
import type { WorkShiftTask } from '../../models/work-shift-task';
import { WorkShiftListFormTypes } from '../../list-form-types';
import type { WorkShiftTasksSectionProps } from './work-shift-tasks-section.types';

/**
 * Список пунктов смены на форме редактирования (веб — таблица).
 */
export function WorkShiftTasksSection({
  listData,
  onCreateTask,
  onTaskRowClick,
  showTitle = true,
  workShiftDate,
}: WorkShiftTasksSectionProps) {
  const [transferTask, setTransferTask] = useState<WorkShiftTask | null>(null);

  const openTransferModal = useCallback((task: WorkShiftTask) => {
    if (!canTransferWorkShiftTask(task)) {
      return;
    }
    setTransferTask(task);
  }, []);

  const transferButtons = useMemo(
    () => [
      {
        name: 'Перенести',
        icon: CalendarDaysIcon,
        isVisible: canTransferWorkShiftTask,
        action: openTransferModal,
      },
    ],
    [openTransferModal],
  );

  return (
    <VStack space="sm" className={showTitle ? 'mt-4' : undefined}>
      <HStack
        className={
          showTitle
            ? 'items-center justify-between'
            : 'items-center justify-end'
        }
      >
        {showTitle ? (
          <Text className="text-base font-medium">Пункты смены</Text>
        ) : null}
        <Button size="sm" onPress={onCreateTask}>
          <ButtonText>Создать пункт</ButtonText>
        </Button>
      </HStack>
      <EntityList
        listFormType={WorkShiftListFormTypes.WorkShiftTask}
        editRoute="work-shifts/tasks"
        listData={listData}
        hasFilter={false}
        hasSearch={false}
        hasSelection={false}
        showDeleteMenuItemInRow={false}
        buttons={transferButtons}
        columns={[
          {
            column: 'plannedStartTime',
            title: 'Плановое начало',
            renderComponent: (task: WorkShiftTask) => (
              <Text>
                {formatPlannedStartTime(task.plannedStartTime)}
              </Text>
            ),
          },
          {
            column: 'actualStartTime',
            title: 'Фактическое начало',
          },
          {
            column: 'actualEndTime',
            title: 'Фактическое окончание',
          },
          {
            column: 'workShiftStatus.name',
            title: 'Статус',
          },
        ]}
        onRowClick={onTaskRowClick}
      />
      <TransferWorkShiftTaskModal
        task={transferTask}
        isOpen={transferTask != null}
        workShiftDate={workShiftDate}
        onClose={() => setTransferTask(null)}
        onTransferred={() => listData.refresh()}
      />
    </VStack>
  );
}

import { useState } from 'react';
import { ActivityIndicator, StyleSheet } from 'react-native';
import EntityListPagination from '@tmobis/core/components/entity-list/pagination';
import { getEntityColumnValue } from '@tmobis/core/utils/entity-column-value';
import { Box } from '@tmobis/core/components/ui/box';
import { Button, ButtonText } from '@tmobis/core/components/ui/button';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { TransferWorkShiftTaskModal } from '../../components/transfer-work-shift-task-modal';
import { formatPlannedStartTime } from '../../utils/planned-start-time';
import { canTransferWorkShiftTask } from '../../utils/work-shift-task-transfer';
import type { WorkShiftTask } from '../../models/work-shift-task';
import type { WorkShiftTasksSectionProps } from './work-shift-tasks-section.types';

/** Фон карточки в духе mobile OLV Ember. */
const CARD_BACKGROUND = '#E8F0C8';
const CARD_BORDER = '#C5D49A';

type TaskCardField = {
  title: string;
  value: string;
};

function getTaskCardFields(task: WorkShiftTask): TaskCardField[] {
  const record = task as Record<string, unknown>;

  return [
    {
      title: 'Плановое начало',
      value: formatPlannedStartTime(task.plannedStartTime) || '—',
    },
    {
      title: 'Фактическое начало',
      value: getEntityColumnValue(record, 'actualStartTime') || '—',
    },
    {
      title: 'Фактическое окончание',
      value: getEntityColumnValue(record, 'actualEndTime') || '—',
    },
    {
      title: 'Статус',
      value: getEntityColumnValue(record, 'workShiftStatus.name') || '—',
    },
  ];
}

/**
 * Нативный список пунктов: карточки label/value, как mobile OLV в Ember.
 */
export function WorkShiftTasksSection({
  listData,
  onCreateTask,
  onTaskRowClick,
  showTitle = true,
  workShiftDate,
}: WorkShiftTasksSectionProps) {
  const { data, isLoading, refresh, paging } = listData;
  const [transferTask, setTransferTask] = useState<WorkShiftTask | null>(null);
  const totalPages = Math.max(
    1,
    Math.ceil((paging.totalCount ?? 0) / paging.perPage) || 1
  );
  const showPaging = totalPages > 1;

  return (
    <VStack space="md" className={showTitle ? 'mt-2' : undefined}>
      <HStack
        className={
          showTitle
            ? 'items-center justify-between'
            : 'items-center justify-end gap-2'
        }
      >
        {showTitle ? (
          <Text className="text-base font-medium">Пункты смены</Text>
        ) : null}
        <HStack className="items-center gap-2">
          <Button size="sm" action="secondary" onPress={() => refresh()}>
            <ButtonText>Обновить</ButtonText>
          </Button>
          <Button size="sm" onPress={onCreateTask}>
            <ButtonText>Создать пункт</ButtonText>
          </Button>
        </HStack>
      </HStack>

      {isLoading ? (
        <VStack className="items-center py-8">
          <ActivityIndicator size="large" color="#208AEF" />
        </VStack>
      ) : !data?.length ? (
        <Text className="py-6 text-center text-muted-foreground">
          Нет пунктов смены
        </Text>
      ) : (
        <VStack space="md">
          {data.map((task) => (
            <Box key={task.id} style={styles.card}>
              <Pressable
                onPress={() => onTaskRowClick(task)}
                accessibilityRole="button"
                accessibilityLabel="Открыть пункт смены"
              >
                <VStack space="md">
                  {getTaskCardFields(task).map((field) => (
                    <VStack key={field.title} space="xs">
                      <Text className="text-xs text-muted-foreground">
                        {field.title}
                      </Text>
                      <Text className="text-base text-typography-900">
                        {field.value}
                      </Text>
                    </VStack>
                  ))}
                </VStack>
              </Pressable>
              {canTransferWorkShiftTask(task) ? (
                <HStack className="mt-3 justify-end">
                  <Button
                    size="sm"
                    action="secondary"
                    onPress={() => setTransferTask(task)}
                  >
                    <ButtonText>Перенести</ButtonText>
                  </Button>
                </HStack>
              ) : null}
            </Box>
          ))}
        </VStack>
      )}

      {showPaging ? <EntityListPagination paging={paging} /> : null}

      <TransferWorkShiftTaskModal
        task={transferTask}
        isOpen={transferTask != null}
        workShiftDate={workShiftDate}
        onClose={() => setTransferTask(null)}
        onTransferred={() => refresh()}
      />
    </VStack>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: CARD_BACKGROUND,
    borderColor: CARD_BORDER,
    borderWidth: 1,
    borderRadius: 8,
    paddingHorizontal: 16,
    paddingVertical: 12,
  },
});

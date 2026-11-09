import { useCallback, useEffect, useMemo, useState } from 'react';
import dayjs from '@tmobis/core/initializers/dayjs';
import { useApiClient } from '@tmobis/core/api/api-client-context';
import { createMessageApi } from '@tmobis/core/api/services/message-api';
import { TmobisSimpleDatetime } from '@tmobis/core/components/datetime';
import { TmobisModal } from '@tmobis/core/components/modal';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { TransferWorkShiftTask } from '../messages/transfer-work-shift-task';
import type { WorkShiftTask } from '../models/work-shift-task';
import { canTransferWorkShiftTask } from '../utils/work-shift-task-transfer';

type TransferWorkShiftTaskModalProps = {
  task: WorkShiftTask | null;
  isOpen: boolean;
  onClose: () => void;
  onTransferred: () => void | Promise<void>;
  /** Дата текущей смены (YYYY-MM-DD), для нижней границы выбора. */
  workShiftDate?: string | null;
};

/**
 * Модальное окно выбора даты переноса пункта смены.
 */
export function TransferWorkShiftTaskModal({
  task,
  isOpen,
  onClose,
  onTransferred,
  workShiftDate,
}: TransferWorkShiftTaskModalProps) {
  const client = useApiClient();
  const messageApi = useMemo(() => createMessageApi(client), [client]);

  const minimumDate = useMemo(() => {
    const tomorrow = dayjs().add(1, 'day').startOf('day');
    const afterShift = workShiftDate
      ? dayjs(workShiftDate).add(1, 'day').startOf('day')
      : null;
    const min = afterShift && afterShift.isAfter(tomorrow) ? afterShift : tomorrow;
    return min.toDate();
  }, [workShiftDate]);

  const [targetDate, setTargetDate] = useState<Date | undefined>(undefined);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    setTargetDate(minimumDate);
    setErrorMessage(null);
    setIsSubmitting(false);
  }, [isOpen, minimumDate]);

  const resetAndClose = useCallback(() => {
    setErrorMessage(null);
    setIsSubmitting(false);
    onClose();
  }, [onClose]);

  const handleApprove = useCallback(async () => {
    if (!task?.id) {
      return;
    }

    if (!canTransferWorkShiftTask(task)) {
      setErrorMessage(
        'Нельзя перенести пункт смены в статусе «Перенесено» или «Выполнено».',
      );
      return;
    }

    if (!targetDate) {
      setErrorMessage('Необходимо задать дату переноса.');
      return;
    }

    const target = dayjs(targetDate).startOf('day');
    const today = dayjs().startOf('day');
    if (!target.isAfter(today)) {
      setErrorMessage('Дата переноса должна быть больше текущей даты.');
      return;
    }

    if (workShiftDate) {
      const shiftDay = dayjs(workShiftDate).startOf('day');
      if (!target.isAfter(shiftDay)) {
        setErrorMessage('Дата переноса должна быть больше даты текущей смены.');
        return;
      }
    }

    setIsSubmitting(true);
    setErrorMessage(null);

    try {
      const result = await messageApi.sendMessage(
        new TransferWorkShiftTask({
          workShiftTaskId: task.id,
          targetDate: target.format('YYYY-MM-DD'),
        }),
      );

      if (result?.error) {
        setErrorMessage(
          result.message || 'Ошибка при переносе пункта смены.',
        );
        setIsSubmitting(false);
        return;
      }

      await onTransferred();
      resetAndClose();
    } catch (error) {
      console.error('Ошибка переноса пункта смены:', error);
      setErrorMessage('Ошибка при переносе пункта смены.');
      setIsSubmitting(false);
    }
  }, [
    messageApi,
    onTransferred,
    resetAndClose,
    targetDate,
    task,
    workShiftDate,
  ]);

  return (
    <TmobisModal
      isOpen={isOpen}
      onClose={resetAndClose}
      title="Перенос на другую дату"
      size="md"
      isLoading={isSubmitting}
      isDenyVisible
      onDeny={resetAndClose}
      onDenyTitle="Отменить"
      isApproveVisible
      onApprove={() => {
        void handleApprove();
      }}
      onApproveTitle="Перенести"
    >
      <VStack space="md">
        <VStack space="xs">
          <Text className="text-sm text-typography-700">Дата переноса</Text>
          <TmobisSimpleDatetime
            mode="date"
            value={targetDate}
            minimumDate={minimumDate}
            onChange={(date) => {
              setTargetDate(date);
              setErrorMessage(null);
            }}
            placeholder="Выберите дату"
          />
        </VStack>
        {errorMessage ? (
          <Text className="text-sm text-error-600">{errorMessage}</Text>
        ) : null}
      </VStack>
    </TmobisModal>
  );
}

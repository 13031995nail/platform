import { router } from 'expo-router';
import { getCurrentEmployeeId } from '@tmobis/auth-keycloak/lib/auth-service';
import type { MessageApi } from '@tmobis/core/api/services/message-api';
import { DateOnly } from '@tmobis/core/utils/date';
import { GetTodayWorkShiftId } from '../messages/get-today-work-shift-id';

const getTodayWorkShiftMessage = new GetTodayWorkShiftId();

export async function navigateToTodayWorkShift(messageApi: MessageApi) {
  const employeeId = await getCurrentEmployeeId();
  if (!employeeId) {
    alert('Не найден сотрудник для текущего пользователя');
    return;
  }

  try {
    const result = await messageApi.sendMessage(getTodayWorkShiftMessage);

    if (result.workShiftId) {
      router.navigate(`/work-shifts/${result.workShiftId}`);
      return;
    }

    router.navigate({
      pathname: '/work-shifts/new',
      params: {
        date: new DateOnly().toISOString(),
        employeeId,
      },
    });
  } catch (error) {
    console.error('Ошибка получения сегодняшней смены:', error);
    alert('Не удалось получить данные о смене');
  }
}

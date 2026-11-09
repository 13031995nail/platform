import dayjs from '@tmobis/core/initializers/dayjs';

/**
 * Получение даты смены из связанной сущности или запасного значения.
 */
export function getWorkShiftDate(
  workShift?: { date?: string | null } | null,
  fallback?: string | null,
): string {
  return workShift?.date ?? fallback ?? '';
}

/**
 * Объединение даты смены и выбранного времени в ISO DateTime.
 */
export function combineShiftDateAndTime(
  shiftDate: string | undefined | null,
  timeValue: Date | string | undefined | null,
): string | undefined {
  if (!timeValue || !shiftDate) {
    return undefined;
  }

  const time = dayjs(timeValue);
  if (!time.isValid()) {
    return undefined;
  }

  const base = dayjs(shiftDate, 'YYYY-MM-DD', true).isValid()
    ? dayjs(shiftDate, 'YYYY-MM-DD')
    : dayjs(shiftDate);

  if (!base.isValid()) {
    return undefined;
  }

  return base
    .hour(time.hour())
    .minute(time.minute())
    .second(0)
    .millisecond(0)
    .toISOString();
}

/**
 * Форматирование планового времени начала для отображения (только время).
 */
export function formatPlannedStartTime(
  value: string | null | undefined,
): string {
  if (!value) {
    return '';
  }

  const parsed = dayjs(value);
  return parsed.isValid() ? parsed.format('HH:mm') : '';
}

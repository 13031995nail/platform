import dayjs from '@tmobis/core/initializers/dayjs';

const ISO_DATE_PATTERN = /^\d{4}-\d{2}-\d{2}/;
const ISO_DATETIME_PATTERN =
  /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?::\d{2}(?:\.\d{1,3})?)?(?:Z|[+-]\d{2}:?\d{2})?$/;

function formatDisplayValue(value: unknown): string | undefined {
  if (value === null || value === undefined) {
    return undefined;
  }

  if (typeof value === 'boolean') {
    return value ? 'Да' : 'Нет';
  }

  if (typeof value === 'string') {
    if (ISO_DATETIME_PATTERN.test(value)) {
      return dayjs(value).format('DD.MM.YYYY HH:mm');
    }

    if (ISO_DATE_PATTERN.test(value)) {
      return dayjs(value).format('DD.MM.YYYY');
    }

    return value;
  }

  if (value instanceof Date) {
    return dayjs(value).format('DD.MM.YYYY HH:mm');
  }

  if (typeof value === 'object') {
    return undefined;
  }

  return String(value);
}

export function getEntityColumnValue(
  entity: Record<string, unknown>,
  column: string
): string | undefined {
  const value = column.split('.').reduce<unknown>((current, part) => {
    if (current === null || current === undefined) {
      return undefined;
    }

    return (current as Record<string, unknown>)[part];
  }, entity);

  return formatDisplayValue(value);
}

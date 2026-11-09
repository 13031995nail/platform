export const firstLetterToLower = (str: string | undefined | null) => {
  if (!str) {
    return '';
  }

  return str.charAt(0).toLowerCase() + str.slice(1);
};

export function unknownToString(value: unknown): string {
  if (value === null || value === undefined) {
    return '';
  }
  if (typeof value === 'string') {
    return value;
  }
  if (typeof value === 'number' || typeof value === 'boolean') {
    return String(value);
  }
  if (value instanceof Date) {
    return value.toISOString();
  }

  try {
    return JSON.stringify(value);
  } catch {
    return String(value);
  }
}

export function isJsonString(value: unknown): value is string {
  if (typeof value !== 'string') {
    return false;
  }
  try {
    const parsed = JSON.parse(value);
    // JSON может быть числом или строкой, поэтому проверим что это объект/массив.
    return typeof parsed === 'object' && parsed !== null;
  } catch {
    return false;
  }
}

export function isNullOrWhiteSpace(value: string | undefined | null) {
  return value === null || value === undefined || value.replace(/\s/g, '').length === 0;
}

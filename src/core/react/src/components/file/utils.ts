/**
 * Преобразует сроку value в массив.
 */
export function getIdsFromString(
  value: string | undefined,
  fallback?: string[]
) {
  if (!value) {
    return fallback;
  }
  
  return value.split(';');
}
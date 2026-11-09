export function normalizePositiveIntegerParam(
  value: string | string[] | undefined,
  fallback?: number
) {
  const normalizedValue = Array.isArray(value) ? value[0] : value;
  const parsedValue = Number(normalizedValue);

  if (!Number.isInteger(parsedValue) || parsedValue < 1) {
    return fallback;
  }

  return parsedValue;
}

/**
 * Формирование списка видимых номеров страниц с многоточием.
 */
export function getVisiblePages(
  currentPage: number,
  totalPages: number
): Array<number | 'ellipsis'> {
  if (totalPages <= 1) {
    return [1];
  }

  if (totalPages <= 7) {
    return Array.from({ length: totalPages }, (_, index) => index + 1);
  }

  const pages = new Set<number>();
  pages.add(1);
  pages.add(totalPages);

  const windowStart = Math.max(2, currentPage - 1);
  const windowEnd = Math.min(totalPages - 1, currentPage + 2);
  for (let page = windowStart; page <= windowEnd; page += 1) {
    pages.add(page);
  }

  if (currentPage <= 2) {
    pages.add(2);
    pages.add(3);
    pages.add(4);
  }

  const sorted = [...pages].sort((a, b) => a - b);
  const result: Array<number | 'ellipsis'> = [];

  sorted.forEach((page, index) => {
    if (index > 0 && page - sorted[index - 1] > 1) {
      result.push('ellipsis');
    }
    result.push(page);
  });

  return result;
}

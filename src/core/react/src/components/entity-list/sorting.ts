export function getNextSorting(currentSorting: string | undefined, column: string) {
  const propertyName = getSortingPropertyName(column);

  if (currentSorting === propertyName) {
    return `-${propertyName}`;
  }

  if (currentSorting === `-${propertyName}`) {
    return undefined;
  }

  return propertyName;
}

export function getSortingIndicator(sorting: string | undefined, column: string) {
  const propertyName = getSortingPropertyName(column);

  if (sorting === propertyName) {
    return " ↑";
  }

  if (sorting === `-${propertyName}`) {
    return " ↓";
  }

  return "";
}

export function normalizeSortingParam(sorting: string | string[] | undefined) {
  const normalizedSorting = Array.isArray(sorting) ? sorting[0] : sorting;

  return normalizedSorting || undefined;
}

function getSortingPropertyName(value: string) {
  return value.charAt(0).toUpperCase() + value.slice(1);
}

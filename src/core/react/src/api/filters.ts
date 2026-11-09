export type FilterOperator =
  | 'eq'
  | 'neq'
  | 'contains'
  | 'gte'
  | 'lte'
  | 'gt'
  | 'lt';

export interface ListFilterRangeValue {
  gte?: string;
  lte?: string;
  gt?: string;
  lt?: string;
}

export type ListFilterFieldValue = string | ListFilterRangeValue | undefined;

export interface ListFilterValues {
  [key: string]: ListFilterFieldValue;
}

const RANGE_OPERATORS: (keyof ListFilterRangeValue)[] = ['gte', 'lte', 'gt', 'lt'];

export function isListFilterRangeValue(
  value: ListFilterFieldValue
): value is ListFilterRangeValue {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
}

export function fromUrlFilter(filterParams: string[] | string): ListFilterValues {
  const filter: ListFilterValues = {};
  const filterArray = typeof filterParams === 'string'
    ? [filterParams]
    : filterParams;

  filterArray.forEach(filterParam => {
    const split = filterParam.split(';');
    if (split.length < 3) {
      return;
    }

    const key = split[0];
    const operator = split[1] as FilterOperator;
    const value = split.length > 3
      ? split.slice(2).join(';')
      : split[2];

    if (RANGE_OPERATORS.includes(operator as keyof ListFilterRangeValue)) {
      const current = isListFilterRangeValue(filter[key])
        ? filter[key]
        : {};
      filter[key] = {
        ...current,
        [operator]: value,
      };
      return;
    }

    if (operator === 'eq') {
      filter[key] = value;
    }
  });

  return filter;
}

export const toUrlFilter = (filter: ListFilterValues): string[] => {
  const result: string[] = [];

  Object.entries(filter).forEach(([key, value]) => {
    if (value === undefined || value === null || value === '') {
      return;
    }

    if (isListFilterRangeValue(value)) {
      RANGE_OPERATORS.forEach(operator => {
        const boundValue = value[operator];
        if (boundValue !== undefined && boundValue !== null && boundValue !== '') {
          result.push(`${key};${operator};${boundValue}`);
        }
      });
      return;
    }

    result.push(`${key};eq;${value}`);
  });

  return result;
};

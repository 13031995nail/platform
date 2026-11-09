import { TmobisSimpleDatetime } from '@tmobis/core/components/datetime';
import { serializeDate } from '@tmobis/core/components/datetime/use-datetime';
import { TmobisFormControlLabel } from '@tmobis/core/components/form-control';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import dayjs from '@tmobis/core/initializers/dayjs';
import { useCallback } from 'react';
import {
  isListFilterRangeValue,
  ListFilterRangeValue,
} from '../../api/filters';
import { HStack } from '../ui/hstack';
import { useEntityListContext } from './entity-list-context';

const DATE_FORMAT = 'YYYY-MM-DD';
const FILTER_DATETIME_FORMAT = 'YYYY-MM-DDTHH:mm:ss';
const RANGE_OPERATORS: (keyof ListFilterRangeValue)[] = ['gte', 'lte', 'gt', 'lt'];

export interface EntityListDateRangeFilterProps {
  field: string;
  labelFrom?: string;
  labelTo?: string;
}

function toFilterDateFrom(date: string | Date | null | undefined): string | undefined {
  if (!date) {
    return undefined;
  }

  const parsed = dayjs(date, [DATE_FORMAT, FILTER_DATETIME_FORMAT], true);
  const normalized = parsed.isValid() ? parsed : dayjs(date);

  return normalized.isValid()
    ? normalized.startOf('day').format(FILTER_DATETIME_FORMAT)
    : undefined;
}

function toFilterDateTo(date: string | Date | null | undefined): string | undefined {
  if (!date) {
    return undefined;
  }

  const parsed = dayjs(date, [DATE_FORMAT, FILTER_DATETIME_FORMAT], true);
  const normalized = parsed.isValid() ? parsed : dayjs(date);

  return normalized.isValid()
    ? normalized.hour(23).minute(59).second(59).format(FILTER_DATETIME_FORMAT)
    : undefined;
}

function toDatepickerValue(value?: string): string | undefined {
  if (!value) {
    return undefined;
  }

  const parsed = dayjs(value, [FILTER_DATETIME_FORMAT, DATE_FORMAT], true);
  const normalized = parsed.isValid() ? parsed : dayjs(value);

  return normalized.isValid() ? normalized.format(DATE_FORMAT) : undefined;
}

function getRangeValue(
  value: string | ListFilterRangeValue | undefined
): ListFilterRangeValue {
  return isListFilterRangeValue(value) ? value : {};
}

/**
 * Фильтр списка по периоду дат.
 * Передаёт в API два ограничения для одного поля: gte (от) и lte (до).
 */
export default function EntityListDateRangeFilter({
  field,
  labelFrom = 'Дата от',
  labelTo = 'Дата до',
}: EntityListDateRangeFilterProps) {
  const { currentFilter, setFilter } = useEntityListContext().listData.filter;
  const range = getRangeValue(currentFilter[field]);
  const fromValue = toDatepickerValue(range.gte);
  const toValue = toDatepickerValue(range.lte);

  const updateRange = useCallback((nextRange: ListFilterRangeValue) => {
    const hasBounds = RANGE_OPERATORS.some(
      operator => nextRange[operator] !== undefined && nextRange[operator] !== ''
    );
    setFilter(field, hasBounds ? nextRange : undefined);
  }, [field, setFilter]);

  const handleFromChange = useCallback((date: string | Date | null) => {
    const nextRange = { ...range };
    const gte = toFilterDateFrom(date);
    if (gte) {
      nextRange.gte = gte;
    } else {
      delete nextRange.gte;
    }
    updateRange(nextRange);
  }, [range, updateRange]);

  const handleToChange = useCallback((date: string | Date | null) => {
    const nextRange = { ...range };
    const lte = toFilterDateTo(date);
    if (lte) {
      nextRange.lte = lte;
    } else {
      delete nextRange.lte;
    }
    updateRange(nextRange);
  }, [range, updateRange]);

  const handleFromDateChange = useCallback((date?: Date) => {
    handleFromChange(serializeDate(date ?? null, true, 'date') ?? null);
  }, [handleFromChange]);

  const handleToDateChange = useCallback((date?: Date) => {
    handleToChange(serializeDate(date ?? null, true, 'date') ?? null);
  }, [handleToChange]);

  return (
    <HStack className="flex-wrap items-end gap-3">
      <FormControl>
        <TmobisFormControlLabel label={labelFrom} />
        <TmobisSimpleDatetime
          mode="date"
          value={fromValue}
          maximumDate={toValue}
          onChange={handleFromDateChange}
        />
      </FormControl>
      <FormControl>
        <TmobisFormControlLabel label={labelTo} />
        <TmobisSimpleDatetime
          mode="date"
          value={toValue}
          minimumDate={fromValue}
          onChange={handleToDateChange}
        />
      </FormControl>
    </HStack>
  );
}

import { TmobisFormControlLabel } from '@tmobis/core/components/form-control';
import type { IEntity } from '@tmobis/core/models/entity';
import { useCallback, useMemo } from 'react';
import { FormControl } from '../ui/form-control';
import { HStack } from '../ui/hstack';
import { ChevronDownIcon, Icon } from '../ui/icon';
import {
  Select,
  SelectBackdrop,
  SelectContent,
  SelectInput,
  SelectItem,
  SelectPortal,
  SelectTrigger,
} from '../ui/select';
import { Text } from '../ui/text';
import { useEntityListContext } from './entity-list-context';
import useEntityList from './use-entity-list';

export interface EntityListLookupFilterProps<TEntity extends IEntity> {
  field: string;
  label: string;
  placeholder?: string;
  displayField: string;
  entityType: string;
  viewName?: string;
  defaultPerPage?: number;
  /**
   * Компактный вид для системной строки (подпись + шеврон).
   */
  variant?: 'default' | 'toolbar';
  /**
   * Применение фильтра сразу при выборе значения.
   */
  applyOnChange?: boolean;
}

/**
 * Фильтр списка по значению связанной сущности (дропдаун-лукап).
 * Передаёт в API фильтр eq по идентификатору выбранной записи.
 */
export default function EntityListLookupFilter<TEntity extends IEntity>({
  field,
  label,
  placeholder = 'Выберите значение...',
  displayField,
  entityType,
  viewName,
  defaultPerPage = 100,
  variant = 'default',
  applyOnChange = variant === 'toolbar',
}: EntityListLookupFilterProps<TEntity>) {
  const {
    currentFilter,
    currentSearch,
    setFilter,
    applyFilterValues,
  } = useEntityListContext().listData.filter;
  const selectedValueId = typeof currentFilter[field] === 'string'
    ? currentFilter[field]
    : undefined;

  const { data } = useEntityList<TEntity>({
    entityType,
    viewName,
    useUrlParams: false,
    defaultPerPage,
  });

  const selectedEntity = useMemo(
    () => data?.find(entity => entity.id === selectedValueId),
    [data, selectedValueId]
  );

  const selectedLabel = selectedEntity?.[displayField as keyof TEntity] as string | undefined;

  const handleValueChange = useCallback((newValueId: string) => {
    const nextValue = newValueId || undefined;
    setFilter(field, nextValue);

    if (!applyOnChange) {
      return;
    }

    const nextFilter = { ...currentFilter };
    if (nextValue === undefined) {
      delete nextFilter[field];
    } else {
      nextFilter[field] = nextValue;
    }
    applyFilterValues(nextFilter, currentSearch);
  }, [
    applyFilterValues,
    applyOnChange,
    currentFilter,
    currentSearch,
    field,
    setFilter,
  ]);

  if (variant === 'toolbar') {
    return (
      <Select
        selectedValue={selectedValueId}
        onValueChange={handleValueChange}
      >
        <SelectTrigger className="min-h-0 border-0 bg-transparent px-0 py-0 data-[hover=true]:border-0">
          <HStack className="items-center gap-1">
            <Text className="text-sm text-foreground-secondary">
              {selectedLabel || label}
            </Text>
            <Icon as={ChevronDownIcon} size="sm" className="text-foreground-secondary" />
          </HStack>
        </SelectTrigger>
        <SelectPortal>
          <SelectBackdrop />
          <SelectContent>
            <SelectItem value="" label={placeholder || 'Все'} />
            {data?.map(option => (
              <SelectItem
                key={option.id}
                value={option.id ?? ''}
                label={option?.[displayField as keyof TEntity] as string}
              />
            ))}
          </SelectContent>
        </SelectPortal>
      </Select>
    );
  }

  return (
    <FormControl>
      <TmobisFormControlLabel label={label} />
      <Select
        selectedValue={selectedValueId}
        onValueChange={handleValueChange}
      >
        <SelectTrigger>
          <SelectInput
            value={selectedLabel || ''}
            placeholder={placeholder}
          />
        </SelectTrigger>
        <SelectPortal>
          <SelectBackdrop />
          <SelectContent>
            <SelectItem value="" label="Все" />
            {data?.map(option => (
              <SelectItem
                key={option.id}
                value={option.id ?? ''}
                label={option?.[displayField as keyof TEntity] as string}
              />
            ))}
          </SelectContent>
        </SelectPortal>
      </Select>
    </FormControl>
  );
}

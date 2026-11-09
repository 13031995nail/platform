import {
  ListFilterFieldValue,
  ListFilterValues,
} from '../../api/filters';
import type { ReactNode } from 'react';
import { HStack } from '../ui/hstack';
import { useEntityListContext } from './entity-list-context';

interface ListFilterProps {
  children?: (
    filter: ListFilterValues,
    setFilter: (field: string, value?: ListFilterFieldValue) => void
  ) => ReactNode;
}

/**
 * Компонент фильтрации списка сущностей.
 * Подразумевается использование внутри компонента {@link EntityList} для доступа к контексту.
 * Дочерние компоненты получают текущие значения фильтра и функцию для их изменения.
 * Текстовые фильтры отображаются в горизонтальном ряду над системной строкой.
 * @constructor
 */
export default function EntityListFilter({ children }: ListFilterProps) {
  const { currentFilter, setFilter } = useEntityListContext().listData.filter;

  return (
    <HStack className="flex-wrap items-end gap-3">
      {children?.(currentFilter, setFilter)}
    </HStack>
  );
}

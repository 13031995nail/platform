import { useCallback, useEffect, useRef, useState } from "react";
import {
  fromUrlFilter,
  isListFilterRangeValue,
  ListFilterFieldValue,
  ListFilterRangeValue,
  ListFilterValues,
  toUrlFilter,
} from "../../api/filters";
import { useLocalSearchParams, useRouter } from "expo-router";

/**
 * Состояние и функции фильтрации
 */
export interface ListFilterData {
  /**
   * Примененный фильтр
   */
  activeFilter: ListFilterValues;
  
  /**
   * Текущий фильтр на UI
   */
  currentFilter: ListFilterValues;
  
  /**
   * Применение фильтра.
   * Сохранение текущего значения в URL и перезапрос данные из API
   */
  applyFilter: () => void;

  /**
   * Применение переданных значений фильтра (с опциональным поиском).
   */
  applyFilterValues: (filter: ListFilterValues, search?: string) => void;
  
  /**
   * Сброс фильтра.
   * Очистка текущего значения в URL и перезапрос данные из API
   */
  resetFilter: () => void;
  
  /**
   * Установка значения фильтра по полю.
   * Работает в накопительном режиме (по мере заполнения инпута)
   */
  setFilter: (field: string, value?: ListFilterFieldValue) => void;
  
  /**
   * Примененный поиск
   */
  activeSearch?: string;

  /**
   * Текущий поиск на UI
   */
  currentSearch?: string;
  
  /**
   * Установка значения поиска.
   * Работает в накопительном режиме (по мере заполнения инпута)
   */
  setSearch: (value?: string) => void;
}

export interface ListFilterProps {
  /**
   * Событие, вызываемое после применения фильтра
   */
  onFilterApply?: () => void;

  /**
   * Перечень фильтров
   */
  filterFields?: string[];

  /**
   * Хранение параметров списка в URL
   */
  useUrlParams: boolean;

  /**
   * Переопределенные значения фильтрации
   */
  defaultFilter?: ListFilterValues;
}

/**
 * Хук для управления фильтрацией списка сущностей.
 * Состояние фильтра синхронизируется с URL-параметрами.
 * Предоставляются функции установки значения фильтра, их применения и сброса.
 *
 * @param props - Параметры {@link ListFilterProps}
 * @returns Состояние и методы фильтрации {@link ListFilterData}
 *
 * @example
 * ```ts
 * const filter = useEntityListFilter({
 *   onFilterApply: () => {
 *     console.log('Фильтр применён');
 *   }
 * });
 * ```
 */
export default function useEntityListFilter({
  onFilterApply, filterFields, useUrlParams,
  defaultFilter
}: ListFilterProps): ListFilterData {
  const router = useRouter();
  const {
    filter: filterParam,
    search: searchParam
  } = useLocalSearchParams<{
    filter?: string | string[];
    search?: string;
  }>();

  const defaultFilterFinal = filterParam && useUrlParams
    ? fromUrlFilter(filterParam)
    : defaultFilter ?? {};
  const [activeFilter, setActiveFilter] = useState<ListFilterValues>(defaultFilterFinal);
  const [currentFilter, setCurrentFilter] = useState<ListFilterValues>(
    initCurrentFilter(defaultFilterFinal, filterFields));

  const defaultSearch = useUrlParams ? searchParam : undefined;
  const [activeSearch, setActiveSearch] = useState<string | undefined>(defaultSearch);
  const [currentSearch, setCurrentSearch] = useState<string | undefined>(activeSearch || '');

  const urlStateKey = JSON.stringify({
    filter: filterParam ?? null,
    search: searchParam ?? null,
    useUrlParams,
    defaultFilter: toUrlFilter(defaultFilter ?? {}),
    filterFields: filterFields ?? [],
  });
  const lastUrlStateKey = useRef(urlStateKey);

  useEffect(() => {
    if (!useUrlParams || lastUrlStateKey.current === urlStateKey) {
      return;
    }
    lastUrlStateKey.current = urlStateKey;
    const nextFilter = filterParam ? fromUrlFilter(filterParam) : defaultFilter ?? {};
    setActiveFilter(nextFilter);
    setCurrentFilter(initCurrentFilter(nextFilter, filterFields));
    setActiveSearch(searchParam || undefined);
    setCurrentSearch(searchParam || '');
  }, [defaultFilter, filterFields, filterParam, searchParam, urlStateKey, useUrlParams]);

  const applyFilterInternal = useCallback((newFilter: ListFilterValues) => {
    const escaped = initActiveFilter(newFilter);
    setActiveFilter(escaped);
    if (useUrlParams) {
      router.setParams({ filter: toUrlFilter(escaped) });
    }
  }, [router, useUrlParams]);

  const applySearchInternal = useCallback((newSearch: string | undefined) => {
    setActiveSearch(newSearch);
    if (useUrlParams) {
      router.setParams({ search: newSearch });
    }
  }, [router, useUrlParams]);

  const applyFilterValues = useCallback((
    filter: ListFilterValues,
    search?: string
  ) => {
    applyFilterInternal(filter);
    applySearchInternal(search || undefined);
    onFilterApply?.();
  }, [applyFilterInternal, applySearchInternal, onFilterApply]);

  const applyFilter = useCallback(() => {
    applyFilterValues(currentFilter, currentSearch || undefined);
  }, [applyFilterValues, currentFilter, currentSearch]);

  const resetFilter = useCallback(() => {
    const newFilter = defaultFilter ?? {};
    const newSearch = '';
    applyFilterInternal(newFilter);
    applySearchInternal(undefined);
    setCurrentFilter(initCurrentFilter(newFilter, filterFields));
    setCurrentSearch(newSearch);
    onFilterApply?.();
  }, [applyFilterInternal, applySearchInternal, defaultFilter, filterFields, onFilterApply]);

  const setFilter = useCallback((field: string, value?: ListFilterFieldValue) => {
    setCurrentFilter(prev => {
      const next = { ...prev };
      if (value === undefined || value === null || value === '') {
        delete next[field];
        return next;
      }
      return { ...next, [field]: value };
    });
  }, []);

  const setSearch = useCallback((value?: string) => {
    setCurrentSearch(value);
  }, []);

  return {
    currentFilter,
    currentSearch,
    activeFilter,
    activeSearch,
    applyFilter,
    applyFilterValues,
    resetFilter,
    setFilter,
    setSearch,
  };
}

const initCurrentFilter = (
  newFilter: ListFilterValues, filterFields?: string[]
) => {
  const escaped: ListFilterValues = { ...newFilter };
  filterFields?.forEach(field => {
    if (escaped[field] === undefined || escaped[field] === null) {
      escaped[field] = '';
    }
  });
  return escaped;
}

const initActiveFilter = (
  newFilter: ListFilterValues
) => {
  const escaped: ListFilterValues = {};

  Object.entries(newFilter).forEach(([key, value]) => {
    if (value === undefined || value === null || value === '') {
      return;
    }

    if (isListFilterRangeValue(value)) {
      const range = Object.fromEntries(
        Object.entries(value).filter(
          ([_, bound]) => bound !== undefined && bound !== null && bound !== ''
        )
      ) as ListFilterRangeValue;

      if (Object.keys(range).length > 0) {
        escaped[key] = range;
      }
      return;
    }

    escaped[key] = value;
  });

  return escaped;
}

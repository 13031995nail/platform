import { TmobisSimpleInput } from '@tmobis/core/components/input';
import { useCallback, useMemo, useState } from 'react';
import { HStack } from '../ui/hstack';
import {
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  Icon,
} from '../ui/icon';
import { Pressable } from '../ui/pressable';
import {
  Select,
  SelectBackdrop,
  SelectContent,
  SelectItem,
  SelectPortal,
  SelectTrigger,
} from '../ui/select';
import { Text } from '../ui/text';
import { getVisiblePages } from './paging';
import type { EntityListPagingData } from './use-entity-list-paging';

export const ENTITY_LIST_PER_PAGE_OPTIONS = [5, 10, 25, 50, 100];

interface EntityListPaginationProps {
  paging: EntityListPagingData;
}

/**
 * Панель постраничной навигации списковой формы.
 */
export default function EntityListPagination({
  paging,
}: EntityListPaginationProps) {
  const {
    page,
    setPage,
    perPage,
    setPerPage,
    totalCount = 0,
    canGoNext,
  } = paging;

  const totalPages = Math.max(1, Math.ceil(totalCount / perPage) || 1);
  const [jumpPage, setJumpPage] = useState('');

  const visiblePages = useMemo(
    () => getVisiblePages(page, totalPages),
    [page, totalPages]
  );

  const rangeFrom = totalCount === 0 ? 0 : (page - 1) * perPage + 1;
  const rangeTo = Math.min(page * perPage, totalCount);

  const handleJump = useCallback(() => {
    const nextPage = Number(jumpPage);
    if (!Number.isInteger(nextPage) || nextPage < 1 || nextPage > totalPages) {
      return;
    }
    setPage(nextPage);
    setJumpPage('');
  }, [jumpPage, setPage, totalPages]);

  return (
    <HStack className="w-full flex-wrap items-center justify-between gap-3">
      <HStack className="flex-wrap items-center gap-1">
        <Pressable
          disabled={page < 2}
          onPress={() => page > 1 && setPage(page - 1)}
          className="h-8 w-8 items-center justify-center rounded data-[disabled=true]:opacity-40"
        >
          <Icon as={ChevronLeftIcon} size="sm" className="text-foreground-secondary" />
        </Pressable>

        {visiblePages.map((item, index) => {
          if (item === 'ellipsis') {
            return (
              <Text
                key={`ellipsis-${index}`}
                className="px-1 text-sm text-foreground-secondary"
              >
                ...
              </Text>
            );
          }

          const isCurrent = item === page;
          return (
            <Pressable
              key={item}
              onPress={() => setPage(item)}
              className={`min-w-8 h-8 items-center justify-center rounded px-2 ${
                isCurrent ? 'bg-secondary' : ''
              }`}
            >
              <Text
                className={`text-sm ${
                  isCurrent
                    ? 'font-medium text-foreground'
                    : 'text-foreground-secondary'
                }`}
              >
                {item}
              </Text>
            </Pressable>
          );
        })}

        <Pressable
          disabled={!canGoNext}
          onPress={() => canGoNext && setPage(page + 1)}
          className="h-8 w-8 items-center justify-center rounded data-[disabled=true]:opacity-40"
        >
          <Icon as={ChevronRightIcon} size="sm" className="text-foreground-secondary" />
        </Pressable>

        <HStack className="ml-2 w-16 items-center gap-1">
          <TmobisSimpleInput
            placeholder="№ с..."
            value={jumpPage}
            onChangeText={setJumpPage}
            onSubmitEditing={handleJump}
            hiddenClearIcon
          />
        </HStack>
      </HStack>

      <HStack className="items-center gap-3">
        <Text className="text-sm text-foreground-secondary">
          Показано {rangeFrom}-{rangeTo} из {totalCount} записей
        </Text>
        <Select
          selectedValue={String(perPage)}
          onValueChange={(value) => setPerPage(Number(value))}
        >
          <SelectTrigger
            variant="outline"
            size="sm"
            className="min-w-16 gap-1 border-border bg-background px-2"
          >
            <Text className="text-sm text-foreground">{perPage}</Text>
            <Icon as={ChevronDownIcon} size="sm" className="text-foreground-secondary" />
          </SelectTrigger>
          <SelectPortal>
            <SelectBackdrop />
            <SelectContent>
              {ENTITY_LIST_PER_PAGE_OPTIONS.map((option) => (
                <SelectItem
                  key={option}
                  value={String(option)}
                  label={String(option)}
                />
              ))}
            </SelectContent>
          </SelectPortal>
        </Select>
      </HStack>
    </HStack>
  );
}

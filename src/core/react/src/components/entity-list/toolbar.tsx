import { TmobisSimpleCheckbox } from '@tmobis/core/components/checkbox';
import { TmobisButton } from '@tmobis/core/components/button';
import { HStack } from '../ui/hstack';
import { AddIcon, RepeatIcon, TrashIcon } from '../ui/icon';
import {
  EntityListSearch,
  EntityListSystemMenu,
  ToolbarActionButton,
  useEntityListToolbarData,
} from './toolbar-shared';

/**
 * Системная строка списковой формы.
 */
export default function EntityListToolbar() {
  const {
    hasActions,
    hasSelection,
    isAllSelected,
    toggleSelectAll,
    showDeleteButton,
    showAddButton,
    showRefreshButton,
    onAdd,
    toolbarFilters,
    hasSearch,
    hasFilter,
    hasSystemTray,
    visibleMenuItems,
    selectedCount,
    deleteSelected,
    listData: {
      refresh,
      filter: { currentSearch, applyFilter, resetFilter, setSearch },
    },
  } = useEntityListToolbarData();

  if (!hasActions) {
    return null;
  }

  return (
    <HStack className="mb-3 w-full flex-wrap items-center gap-3">
      <HStack className="flex-wrap items-center gap-3">
        {hasSelection && (
          <TmobisSimpleCheckbox
            value="select-all"
            isChecked={isAllSelected}
            onChange={toggleSelectAll}
          />
        )}

        {showDeleteButton && (
          <ToolbarActionButton
            label="Удалить"
            icon={TrashIcon}
            disabled={selectedCount === 0}
            onPress={deleteSelected}
          />
        )}

        {showAddButton && (
          <ToolbarActionButton
            label="Добавить"
            icon={AddIcon}
            onPress={onAdd}
          />
        )}

        {showRefreshButton && (
          <ToolbarActionButton
            label="Обновить"
            icon={RepeatIcon}
            onPress={refresh}
          />
        )}
      </HStack>

      <HStack className="flex-1 flex-wrap items-center gap-4">
        {toolbarFilters}
      </HStack>

      <HStack className="flex-wrap items-center gap-2">
        {hasSystemTray && (
          <EntityListSystemMenu items={visibleMenuItems} />
        )}

        {(hasSearch || hasFilter || Boolean(toolbarFilters)) && (
          <TmobisButton
            label="Сбросить"
            size="sm"
            action="secondary"
            onPress={resetFilter}
          />
        )}

        {hasSearch && (
          <EntityListSearch
            value={currentSearch}
            onChange={setSearch}
            onApply={applyFilter}
          />
        )}
      </HStack>
    </HStack>
  );
}

import { TmobisButton } from '@tmobis/core/components/button';
import { TmobisSimpleCheckbox } from '@tmobis/core/components/checkbox';
import { HStack } from '../ui/hstack';
import { RepeatIcon, TrashIcon } from '../ui/icon';
import { VStack } from '../ui/vstack';
import {
  EntityListSearch,
  EntityListSystemMenu,
  ToolbarActionButton,
  useEntityListToolbarData,
} from './toolbar-shared';

/**
 * Отображение системной строки нативной списковой формы.
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
    hasSystemTray,
    visibleMenuItems,
    selectedCount,
    deleteSelected,
    listData: {
      refresh,
      filter: { currentSearch, applyFilter, setSearch },
    },
  } = useEntityListToolbarData();

  if (!hasActions) {
    return null;
  }

  return (
    <VStack className="mb-3 w-full gap-3">
      {toolbarFilters && (
        <HStack className="flex-wrap items-center gap-3">
          {toolbarFilters}
        </HStack>
      )}

      {hasSearch && (
        <EntityListSearch
          isFullWidth
          value={currentSearch}
          onChange={setSearch}
          onApply={applyFilter}
        />
      )}

      {showAddButton && (
        <TmobisButton
          label="Добавить"
          size="sm"
          action="secondary"
          className="w-full"
          onPress={onAdd}
        />
      )}

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

        {showRefreshButton && (
          <ToolbarActionButton
            label="Обновить"
            icon={RepeatIcon}
            onPress={refresh}
          />
        )}

        {hasSystemTray && (
          <EntityListSystemMenu items={visibleMenuItems} />
        )}
      </HStack>
    </VStack>
  );
}

import { createContext, useContext, type ReactNode } from 'react';
import type { IEntity } from '../../models/entity';
import type {
  EntityListRowConfig,
  EntityListViewButton,
  EntityListViewColumn,
} from './entity-list-types';
import type { EntityListSystemMenuItem } from './toolbar-shared';
import type { EntityListData } from './use-entity-list';

export interface EntityListContextValue<TEntity extends IEntity> {
  listData: EntityListData<TEntity>;
  deleteSelected: () => Promise<void>;
  columns: readonly EntityListViewColumn<TEntity>[];
  rowButtons: EntityListViewButton<TEntity>[];
  hasRowActions: boolean;
  hasSelection: boolean;
  isAllSelected: boolean;
  selectedCount: number;
  isEntitySelected: (entity: TEntity) => boolean;
  setEntitySelected: (entity: TEntity, isSelected: boolean) => void;
  toggleSelectAll: () => void;
  mobileCardExpandable: boolean;
  onRowClick?: (rowData: TEntity) => void;
  resolveRowConfig: (rowData: TEntity) => EntityListRowConfig;
  onAdd?: () => void;
  toolbarFilters?: ReactNode;
  systemMenuItems?: EntityListSystemMenuItem[];
  hasFilter: boolean;
  hasSearch: boolean;
  showDeleteButton: boolean;
  showAddButton: boolean;
  showRefreshButton: boolean;
}

export const EntityListContext = createContext<
  EntityListContextValue<any> | undefined
>(undefined);

export function useEntityListContext<
  TEntity extends IEntity = IEntity
>(): EntityListContextValue<TEntity> {
  const context = useContext(EntityListContext);
  if (!context) {
    throw new Error('useEntityListContext must be used within EntityList');
  }

  return context as EntityListContextValue<TEntity>;
}

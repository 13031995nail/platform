import type { ReactNode } from 'react';
import { TmobisSimpleCheckbox } from '@tmobis/core/components/checkbox';
import type { IEntity } from '@tmobis/core/models/entity';
import { getEntityColumnValue } from '@tmobis/core/utils/entity-column-value';
import { Text } from '../ui/text';
import { useEntityListContext } from './entity-list-context';
import EntityListMobileCard, {
  type EntityListMobileCardField,
} from './mobile-card';
import {
  EntityListRowMenu,
  getColumnContent,
  getVisibleRowButtons,
} from './entity-list-row';
import type { EntityListRowConfig, EntityListViewColumn } from './entity-list-types';

interface RowProps<TEntity extends IEntity> {
  entity: TEntity;
  index: number;
}

function renderCardValue(value: ReactNode): ReactNode {
  if (typeof value === 'string' || typeof value === 'number') {
    return <Text className="text-base text-foreground">{value}</Text>;
  }

  return value;
}

function renderCardTitle(value: ReactNode): ReactNode {
  if (typeof value === 'string' || typeof value === 'number') {
    return (
      <Text bold isTruncated className="text-base text-foreground">
        {value}
      </Text>
    );
  }

  return value ?? <Text className="text-base text-foreground-secondary">—</Text>;
}

function hasCardValue(value: ReactNode): boolean {
  return value !== undefined
    && value !== null
    && value !== ''
    && typeof value !== 'boolean';
}

function getMobileCardTitleColumn<TEntity extends IEntity>(
  columns: readonly EntityListViewColumn<TEntity>[],
  rowConfig: EntityListRowConfig,
): EntityListViewColumn<TEntity> | undefined {
  if (rowConfig.mobileCardTitleColumn) {
    return columns.find(
      column => column.column === rowConfig.mobileCardTitleColumn,
    );
  }

  if (rowConfig.mobileCardTitle !== undefined) {
    return undefined;
  }

  return columns.find(column => column.column === 'name');
}

export default function EntityListBody<TEntity extends IEntity>({ entity, index }: RowProps<TEntity>) {
  const {
    columns, rowButtons, hasSelection, isEntitySelected, setEntitySelected,
    mobileCardExpandable, onRowClick, resolveRowConfig,
  } = useEntityListContext<TEntity>();

  const rowConfig = resolveRowConfig(entity);
  const checkboxKey = `${index}_chk`;
  const titleColumn = getMobileCardTitleColumn(columns, rowConfig);
  const defaultTitleValue = getEntityColumnValue(entity as Record<string, unknown>, 'name') ?? entity.id;
  const titleValue = rowConfig.mobileCardTitle ?? (rowConfig.mobileCardTitleColumn && titleColumn ? getColumnContent(titleColumn, entity) : defaultTitleValue);
  const title = renderCardTitle(hasCardValue(titleValue) ? titleValue : entity.id || undefined);
  const fields = columns.filter(column => column !== titleColumn).flatMap((column, columnIndex): EntityListMobileCardField[] => {
    const value = getColumnContent(column, entity);
    if (!hasCardValue(value)) return [];
    return [{
      key: `${column.column ?? 'column'}_${columnIndex}`,
      column: column.column,
      label: column.title,
      value: renderCardValue(value),
    }];
  });
  const columnsToShow = rowConfig.mobileCardColumnsToShow;
  const visibleFields = columnsToShow ? fields.filter(field => field.column && columnsToShow.includes(field.column)) : fields;
  const collapsedFields = columnsToShow && mobileCardExpandable ? fields.filter(field => !field.column || !columnsToShow.includes(field.column)) : [];
  const visibleButtons = getVisibleRowButtons(rowButtons, rowConfig, entity);
  const selected = isEntitySelected(entity);

  return (
    <EntityListMobileCard
      id={entity.id ?? checkboxKey}
      title={title}
      fields={visibleFields}
      collapsedFields={collapsedFields}
      className={rowConfig.className}
      isSelected={selected}
      checkbox={hasSelection ? (
        <TmobisSimpleCheckbox
          value={entity.id ?? checkboxKey}
          isChecked={selected}
          isDisabled={!entity.id || rowConfig.canBeSelected === false}
          onChange={(isSelected: boolean) => setEntitySelected(entity, isSelected)}
        />
      ) : undefined}
      menu={visibleButtons.length > 0 ? (
        <EntityListRowMenu entity={entity} buttons={visibleButtons} />
      ) : undefined}
      onPress={onRowClick ? () => onRowClick(entity) : undefined}
    />
  );
}
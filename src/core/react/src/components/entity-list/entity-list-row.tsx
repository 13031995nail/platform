import type { ReactNode } from 'react';
import type { IEntity } from '@tmobis/core/models/entity';
import { getEntityColumnValue } from '@tmobis/core/utils/entity-column-value';
import { Icon, ThreeDotsIcon } from '../ui/icon';
import { Menu, MenuItem, MenuItemLabel } from '../ui/menu';
import { Pressable } from '../ui/pressable';
import type {
  EntityListRowConfig,
  EntityListViewButton,
  EntityListViewColumn,
} from './entity-list-types';

export function getColumnContent<TEntity extends IEntity>(
  column: EntityListViewColumn<TEntity>,
  entity: TEntity,
): ReactNode {
  if (column.renderComponent) {
    return column.renderComponent(entity);
  }

  if (column.column) {
    return getEntityColumnValue(entity as Record<string, unknown>, column.column);
  }

  return undefined;
}

export function getVisibleRowButtons<TEntity extends IEntity>(
  buttons: EntityListViewButton<TEntity>[],
  rowConfig: EntityListRowConfig,
  entity: TEntity,
) {
  return buttons.filter(button => {
    const isVisible = typeof button.isVisible === 'function'
      ? button.isVisible(entity)
      : button.isVisible;

    return isVisible
      && !(button.isDeleteAction && rowConfig.canBeDeleted === false);
  });
}

function getDefaultRowConfig<TEntity extends IEntity>(
  entity: TEntity,
): EntityListRowConfig {
  const actuality = (entity as { actuality?: boolean }).actuality;

  return actuality === false
    ? { className: 'bg-background-muted opacity-60' }
    : {};
}

export function resolveRowConfig<TEntity extends IEntity>(
  entity: TEntity,
  getRowConfig?: (entity: TEntity) => EntityListRowConfig,
): EntityListRowConfig {
  const defaultConfig = getDefaultRowConfig(entity);
  const customConfig = getRowConfig?.(entity) ?? {};
  const className = [defaultConfig.className, customConfig.className]
    .filter(Boolean)
    .join(' ');

  return {
    ...defaultConfig,
    ...customConfig,
    className: className || undefined,
  };
}

interface EntityListRowMenuProps<TEntity extends IEntity> {
  entity: TEntity;
  buttons: EntityListViewButton<TEntity>[];
}

export function EntityListRowMenu<TEntity extends IEntity>({
  entity,
  buttons,
}: EntityListRowMenuProps<TEntity>) {
  return (
    <Menu
      placement="top"
      offset={5}
      trigger={({ ...triggerProps }) => (
        <Pressable
          {...triggerProps}
          className="h-8 w-8 items-center justify-center rounded"
          accessibilityLabel="Действия строки"
        >
          <Icon
            as={ThreeDotsIcon}
            size="sm"
            className="rotate-90 text-foreground-secondary"
          />
        </Pressable>
      )}
    >
      {buttons.map((button, index) => (
        <MenuItem
          key={`${index}_menu`}
          textValue={button.name}
          onPress={() => button.action?.(entity)}
        >
          <Icon as={button.icon} size="sm" className="mr-2" />
          <MenuItemLabel>{button.name}</MenuItemLabel>
        </MenuItem>
      ))}
    </Menu>
  );
}

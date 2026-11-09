import { TmobisButton } from '@tmobis/core/components/button';
import { TmobisSimpleInput } from '@tmobis/core/components/input';
import { HStack } from '../ui/hstack';
import { Icon, ThreeDotsIcon, TrashIcon } from '../ui/icon';
import { Menu, MenuItem, MenuItemLabel } from '../ui/menu';
import { Pressable } from '../ui/pressable';
import { Text } from '../ui/text';
import { useEntityListContext } from './entity-list-context';

export interface EntityListSystemMenuItem {
  name: string;
  action?: () => void;
  isVisible?: boolean;
}

export function useEntityListToolbarData() {
  const context = useEntityListContext();
  const visibleMenuItems = (context.systemMenuItems ?? []).filter(
    item => item.isVisible !== false,
  );
  const hasSystemTray = visibleMenuItems.length > 0;
  const hasActions = context.hasSelection
    || context.showDeleteButton
    || context.showAddButton
    || context.showRefreshButton
    || Boolean(context.toolbarFilters)
    || hasSystemTray
    || context.hasSearch;

  return {
    ...context,
    visibleMenuItems,
    hasSystemTray,
    hasActions,
  };
}

interface EntityListSearchProps {
  isFullWidth?: boolean;
  value?: string;
  onChange: (value?: string) => void;
  onApply: () => void;
}

export function EntityListSearch({
  isFullWidth = false,
  value,
  onChange,
  onApply,
}: EntityListSearchProps) {
  return (
    <HStack
      className={isFullWidth
        ? 'w-full items-center gap-2'
        : 'items-center gap-2'}
    >
      <HStack className={isFullWidth ? 'min-w-0 flex-1' : 'w-52'}>
        <TmobisSimpleInput
          placeholder="Поиск..."
          value={value}
          onChangeText={onChange}
          onSubmitEditing={onApply}
          hiddenClearIcon={!value}
        />
      </HStack>
      <TmobisButton
        label="Поиск"
        size="sm"
        action="primary"
        onPress={onApply}
      />
    </HStack>
  );
}

interface EntityListSystemMenuProps {
  items: EntityListSystemMenuItem[];
}

export function EntityListSystemMenu({ items }: EntityListSystemMenuProps) {
  return (
    <Menu
      placement="top"
      offset={5}
      trigger={({ ...triggerProps }) => (
        <Pressable
          {...triggerProps}
          className="h-8 w-8 items-center justify-center rounded"
          accessibilityLabel="Дополнительные действия"
        >
          <Icon
            as={ThreeDotsIcon}
            size="sm"
            className="rotate-90 text-foreground-secondary"
          />
        </Pressable>
      )}
    >
      {items.map((item, index) => (
        <MenuItem
          key={`${item.name}_${index}`}
          textValue={item.name}
          onPress={() => item.action?.()}
        >
          <MenuItemLabel>{item.name}</MenuItemLabel>
        </MenuItem>
      ))}
    </Menu>
  );
}

interface ToolbarActionButtonProps {
  label: string;
  icon: typeof TrashIcon;
  disabled?: boolean;
  onPress?: () => void;
}

export function ToolbarActionButton({
  label,
  icon,
  disabled = false,
  onPress,
}: ToolbarActionButtonProps) {
  return (
    <Pressable
      disabled={disabled}
      onPress={onPress}
      className="flex-row items-center gap-1.5 data-[disabled=true]:opacity-40"
      accessibilityLabel={label}
    >
      <Icon as={icon} size="sm" className="text-foreground-secondary" />
      <Text className="text-sm text-foreground-secondary">{label}</Text>
    </Pressable>
  );
}

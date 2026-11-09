import type {
  DrawerDescriptorMap,
  DrawerNavigationHelpers
}
// @ts-ignore
from "@react-navigation/drawer/src/types";
import {
  DrawerNavigationOptions,
} from '@react-navigation/drawer';
import {
  type DrawerNavigationState,
  type ParamListBase, DrawerActions, CommonActions,
} from '@react-navigation/native';
import { router } from 'expo-router';
import { StyleSheet, Text, View, Pressable } from 'react-native';
import { hasRequiredRole } from '@tmobis/core/security/permissions';
import { sidebarTokens } from './styles';

export type DrawerCustomItemProps = {
  label: string;
  icon?: (props: {
    color: string;
    size: number;
    focused: boolean;
  }) => React.ReactNode;
  roles?: string[];
  disabled?: boolean;
  onPress: (navigation?: DrawerNavigationHelpers) => void | Promise<void>;
};

export type DrawerGroupItemsConfig = {
  group: string;
  items: DrawerCustomItemProps[];
};

export type DrawerGroupProps = {
  label?: string;
  icon?: (props: {
    color: string;
    size: number;
    focused: boolean;
  }) => React.ReactNode;
  roles?: string[];
  routes?: string[];
  items?: DrawerCustomItemProps[];
}

type Props = {
  state: DrawerNavigationState<ParamListBase>;
  descriptors: DrawerDescriptorMap;
  navigation: DrawerNavigationHelpers;
  group: DrawerGroupProps;
  isLast?: boolean;
  activeTintColor?: string;
  activeBackgroundColor?: string;
  inactiveTintColor?: string;
  inactiveBackgroundColor?: string;
};

export type DrawerGroupOptions = DrawerNavigationOptions & {
  roles?: string[];
};

type MenuItemProps = {
  label: string;
  icon?: (props: {
    color: string;
    size: number;
    focused: boolean;
  }) => React.ReactNode;
  focused: boolean;
  disabled?: boolean;
  onPress: () => void;
};

function MenuItem({ label, icon, focused, disabled, onPress }: MenuItemProps) {
  const iconColor = focused
    ? sidebarTokens.labelPrimary
    : sidebarTokens.labelSecondary;

  return (
    <Pressable
      disabled={disabled}
      onPress={onPress}
      // @ts-expect-error hovered доступен в web-реализации Pressable
      style={({ pressed, hovered }) => [
        styles.menuItem,
        focused && styles.menuItemActive,
        (pressed || hovered) && !focused && styles.menuItemHover,
        disabled && styles.menuItemDisabled,
      ]}
    >
      <View style={styles.menuItemContent}>
        {icon ? (
          <View style={styles.menuItemIcon}>
            {icon({
              color: iconColor,
              size: sidebarTokens.iconSize,
              focused,
            })}
          </View>
        ) : null}
        <Text
          style={[
            styles.menuItemLabel,
            focused && styles.menuItemLabelActive,
          ]}
          numberOfLines={1}
        >
          {label}
        </Text>
      </View>
    </Pressable>
  );
}

function SectionLabel({ title }: { title: string }) {
  return (
    <Text style={styles.sectionLabel}>{title}</Text>
  );
}

function MakeDrawerCustomItemsForGroup(
  {
    navigation
  }: Props,
  group: DrawerGroupProps) {
  if (!group.items?.length) {
    return [];
  }

  return group.items.map((item, index) => {
    if (!hasRequiredRole(item.roles)) {
      return null;
    }

    const onPress = () => {
      if (item.disabled) {
        return;
      }

      navigation.dispatch(DrawerActions.closeDrawer());
      void item.onPress(navigation);
    };

    return (
      <MenuItem
        key={`custom-${index}`}
        label={item.label}
        icon={item.icon}
        focused={false}
        disabled={item.disabled}
        onPress={onPress}
      />
    );
  })
    .filter(elem => elem) as React.ReactNode as React.ReactElement[];
}

function MakeDrawerItemsForGroup(
  {
    state, navigation, descriptors
  }: Props,
  group: DrawerGroupProps) {
  return (group.routes ?? []).map((routeName) => {
    const route = state.routes
      .find((route) =>
        route.name === routeName)!;
    if (!route) return null;

    const {
      title,
      drawerLabel,
      drawerIcon,
      roles
    } = descriptors[route.key].options;

    if (!hasRequiredRole(roles)) return null;

    const focused = state.routes.indexOf(route) === state.index;
    const label = drawerLabel !== undefined
      ? typeof drawerLabel === 'string'
        ? drawerLabel
        : title !== undefined
          ? title
          : route.name
      : title !== undefined
        ? title
        : route.name;

    const onPress = () => {
      const event = navigation.emit({
        type: 'drawerItemPress',
        target: route.key,
        canPreventDefault: true,
      });

      if (!event.defaultPrevented) {
        if (focused) {
          router.navigate(`/${route.name}` as never);
          navigation.dispatch(DrawerActions.closeDrawer());
          return;
        }

        navigation.dispatch({
          ...CommonActions.navigate({
            name: route.name,
            params: { screen: 'index' },
          }),
          target: state.key,
        });
      }
    };

    return (
      <MenuItem
        key={route.key}
        label={String(label)}
        icon={drawerIcon}
        focused={focused}
        onPress={onPress}
      />
    );
  })
    .filter(elem => elem) as React.ReactNode as React.ReactElement[];
}

export function DrawerGroup(props: Props) {
  const customItems = MakeDrawerCustomItemsForGroup(props, props.group);
  const routeItems = MakeDrawerItemsForGroup(props, props.group);
  const hasItems = customItems.length > 0 || (Array.isArray(routeItems) && routeItems.length > 0);

  if (!hasItems && !props.group.label) {
    return null;
  }

  return (
    <View style={[styles.group, props.isLast && styles.groupLast]}>
      {props.group.label ? <SectionLabel title={props.group.label} /> : null}
      {customItems}
      {routeItems}
    </View>
  );
}

const styles = StyleSheet.create({
  group: {
    paddingBottom: 8,
    marginBottom: 12,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: sidebarTokens.divider,
  },
  groupLast: {
    borderBottomWidth: 0,
    marginBottom: 0,
  },
  sectionLabel: {
    fontFamily: sidebarTokens.fontFamily,
    fontSize: sidebarTokens.groupTitleSize,
    fontWeight: '500',
    lineHeight: sidebarTokens.groupTitleLineHeight,
    letterSpacing: -0.18,
    color: sidebarTokens.labelSecondary,
    paddingBottom: 8,
  },
  menuItem: {
    borderRadius: sidebarTokens.itemRadius,
    backgroundColor: 'transparent',
  },
  menuItemHover: {
    backgroundColor: sidebarTokens.menuitemHover,
  },
  menuItemActive: {
    backgroundColor: sidebarTokens.menuitemActive,
  },
  menuItemDisabled: {
    opacity: 0.5,
  },
  menuItemContent: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: sidebarTokens.itemGap,
    paddingVertical: sidebarTokens.itemPaddingVertical,
    paddingHorizontal: sidebarTokens.itemPaddingHorizontal,
  },
  menuItemIcon: {
    width: sidebarTokens.iconSize,
    height: sidebarTokens.iconSize,
    alignItems: 'center',
    justifyContent: 'center',
    opacity: 0.85,
  },
  menuItemLabel: {
    flex: 1,
    fontFamily: sidebarTokens.fontFamily,
    fontSize: sidebarTokens.itemFontSize,
    fontWeight: '400',
    lineHeight: sidebarTokens.itemLineHeight,
    color: sidebarTokens.labelPrimary,
  },
  menuItemLabelActive: {
    fontWeight: '500',
  },
});
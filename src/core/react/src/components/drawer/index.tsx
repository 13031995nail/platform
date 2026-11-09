import {
  DrawerContentScrollView
} from '@react-navigation/drawer';
import {
  type DrawerNavigationState,
  type ParamListBase
} from '@react-navigation/native';
import type { DrawerDescriptorMap, DrawerNavigationHelpers }
// @ts-ignore
from "@react-navigation/drawer/src/types";
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { Feather } from '@expo/vector-icons';
import { DrawerGroup, DrawerGroupProps } from './drawer-group';
import { sidebarTokens } from './styles';

type Props = {
  state: DrawerNavigationState<ParamListBase>;
  descriptors: DrawerDescriptorMap;
  navigation: DrawerNavigationHelpers;
  groups?: DrawerGroupProps[];
  onHide?: () => void;
};

/**
 * Контент главного бокового меню: кнопка скрытия и группы разделов.
 */
export function DrawerContent(props: Props) {
  const groups = props.groups ?? [];

  return (
    <View style={styles.root}>
      <View style={styles.hideSegment}>
        <Pressable
          onPress={props.onHide}
          style={({ pressed, hovered }) => [
            styles.hideButton,
            (pressed || hovered) && styles.hideButtonHover,
          ]}
          accessibilityRole="button"
          accessibilityLabel="Скрыть меню"
        >
          <Feather
            name="chevrons-left"
            size={sidebarTokens.iconSize}
            color={sidebarTokens.labelPrimary}
          />
          <Text style={styles.hideButtonLabel}>Скрыть меню</Text>
        </Pressable>
      </View>

      <DrawerContentScrollView
        {...props}
        contentContainerStyle={styles.sitemap}
        style={styles.scroll}
      >
        {groups.map((group: DrawerGroupProps, idx: number) => (
          <DrawerGroup
            {...props}
            group={group}
            key={idx}
            isLast={idx === groups.length - 1}
          />
        ))}
      </DrawerContentScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: sidebarTokens.background,
  },
  hideSegment: {
    height: sidebarTokens.headerSegmentHeight,
    paddingHorizontal: sidebarTokens.sitemapPadding,
    justifyContent: 'center',
  },
  hideButton: {
    height: sidebarTokens.buttonHeight,
    borderRadius: sidebarTokens.buttonRadius,
    backgroundColor: sidebarTokens.buttonSecondary,
    borderWidth: 0,
    outlineStyle: 'none',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    gap: 6,
    paddingHorizontal: 14,
  },
  hideButtonHover: {
    backgroundColor: sidebarTokens.buttonSecondaryHover,
  },
  hideButtonLabel: {
    fontFamily: sidebarTokens.fontFamily,
    fontSize: 16,
    fontWeight: '500',
    lineHeight: 20,
    color: sidebarTokens.labelPrimary,
  },
  scroll: {
    flex: 1,
    backgroundColor: sidebarTokens.background,
  },
  sitemap: {
    paddingHorizontal: sidebarTokens.sitemapPadding,
    paddingBottom: sidebarTokens.sitemapPadding,
    paddingTop: 0,
  },
});

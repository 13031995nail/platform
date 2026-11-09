import React, { useMemo } from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import { useRouter } from 'expo-router';
import { VStack } from '../ui/vstack';
import { HStack } from '../ui/hstack';
import { Text } from '../ui/text';
import { Heading } from '../ui/heading';
import { Pressable } from '../ui/pressable';
import { Divider } from '../ui/divider';
import type { DrawerGroupProps } from '../drawer/drawer-group';
import ScreenData from '../../routes/screen-data';
import { filterScreensByRoles } from '../../routes/filter-screens-by-roles';
import { getCurrentUserRoles } from '../../security/permissions';

type Props = {
  groups: DrawerGroupProps[];
  screens: ScreenData[];
};

/**
 * Меню дополнительных разделов для вкладки "Ещё" на апк.
 */
export function MoreMenu({ groups, screens }: Props) {
  const router = useRouter();
  const visibleScreens = useMemo(() => filterScreensByRoles(screens), [screens]);
  const screenByRoute = useMemo(
    () => new Map(visibleScreens.map((screen) => [screen.route, screen])),
    [visibleScreens]
  );

  return (
    <ScrollView style={styles.container}>
      <VStack className="px-4 py-4 gap-6">
        {groups.map((group, groupIndex) => {
          const routeItems = (group.routes ?? [])
            .map((route) => screenByRoute.get(route))
            .filter((screen): screen is ScreenData => Boolean(screen));

          const customItems = (group.items ?? []).filter((item) => {
            if (!item.roles?.length) {
              return true;
            }

            const userRoles = getCurrentUserRoles();
            return item.roles.some((role) => userRoles.includes(role));
          });

          if (routeItems.length === 0 && customItems.length === 0) {
            return null;
          }

          return (
            <VStack key={group.label ?? groupIndex} className="gap-1">
              {group.label ? (
                <Text className="px-2 mb-1 text-xs font-semibold text-typography-500 uppercase">
                  {group.label}
                </Text>
              ) : null}

              {routeItems.map((screen) => (
                <Pressable
                  key={screen.route}
                  onPress={() => router.navigate(`/${screen.route}` as never)}
                  className="rounded-lg px-3 py-3 active:bg-background-100"
                >
                  <HStack className="items-center gap-3">
                    {screen.icon?.({ color: '#6B7280', size: 22, focused: false })}
                    <Text className="text-base text-typography-900">{screen.label}</Text>
                  </HStack>
                </Pressable>
              ))}

              {customItems.map((item, itemIndex) => (
                <Pressable
                  key={`custom-${item.label}-${itemIndex}`}
                  disabled={item.disabled}
                  onPress={() => {
                    void item.onPress();
                  }}
                  className="rounded-lg px-3 py-3 active:bg-background-100"
                >
                  <HStack className="items-center gap-3">
                    {item.icon?.({ color: '#6B7280', size: 22, focused: false })}
                    <Text className="text-base text-typography-900">{item.label}</Text>
                  </HStack>
                </Pressable>
              ))}

              {groupIndex < groups.length - 1 ? <Divider className="mt-3" /> : null}
            </VStack>
          );
        })}

        {groups.length === 0 ? (
          <Heading size="sm" className="text-typography-500">
            Нет дополнительных разделов
          </Heading>
        ) : null}
      </VStack>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
});

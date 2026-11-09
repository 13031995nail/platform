import { DrawerContent } from '@tmobis/core/components/drawer';
import { sidebarTokens } from '@tmobis/core/components/drawer/styles';
import { DynamicHeader, HeaderSystemControls } from '@tmobis/core/components/header';
import { HeaderProvider } from '@tmobis/core/components/header/context';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { Icon, MenuIcon } from '@tmobis/core/components/ui/icon';
import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import { useModules } from '@tmobis/core/modules/provider';
import { filterScreensByRoles } from '@tmobis/core/routes/filter-screens-by-roles';
import { Drawer } from 'expo-router/drawer';
import React, { useCallback, useMemo, useState } from 'react';
import { Pressable, StyleSheet } from 'react-native';

/**
 * Корневой layout веба: drawer со всеми разделами по группам.
 */
export default function WebDrawerLayout() {
  const { compositionRoot } = useModules();
  const { screens, groups } = compositionRoot.getNavigationPlan();
  const [isSidebarVisible, setIsSidebarVisible] = useState(false);
  const colors = useGluestackColors();

  const headerActions = useMemo(
    () => compositionRoot.getHeaderActions(),
    [compositionRoot],
  );

  const visibleScreens = useMemo(
    () => filterScreensByRoles(screens.filter((screen) => !screen.hideInNav)),
    [screens],
  );

  const hiddenScreens = useMemo(
    () => screens.filter((screen) => screen.hideInNav),
    [screens],
  );

  const onHideSidebar = useCallback(() => {
    setIsSidebarVisible(false);
  }, []);

  const onShowSidebar = useCallback(() => {
    setIsSidebarVisible(true);
  }, []);

  return (
    <HeaderProvider>
      <Drawer
        drawerContent={(props: any) => (
          <DrawerContent {...props} groups={groups} onHide={onHideSidebar} />
        )}
        screenOptions={{
          headerTitle: isSidebarVisible ? () => null : () => <DynamicHeader />,
          headerLeft: !isSidebarVisible
            ? () => (
                <Pressable
                  onPress={onShowSidebar}
                  style={styles.showMenuButton}
                  accessibilityRole='button'
                  accessibilityLabel='Показать меню'
                >
                  <Icon as={MenuIcon} size='md' className='text-foreground' />
                </Pressable>
              )
            : () => null,
          headerRight: () => (
            <HStack className='items-center'>
              {headerActions}
              <HeaderSystemControls />
            </HStack>
          ),
          drawerType: 'permanent',
          sceneStyle: {
            backgroundColor: colors.background,
          },
          headerStyle: {
            backgroundColor: colors.background,
            elevation: 0,
            shadowOpacity: 0,
            borderBottomWidth: StyleSheet.hairlineWidth,
            borderBottomColor: colors.border,
          },
          headerTitleStyle: {
            fontFamily: sidebarTokens.fontFamily,
            fontWeight: '500',
            color: colors.foreground,
          },
          drawerStyle: {
            width: isSidebarVisible ? sidebarTokens.width : 0,
            backgroundColor: colors.sidebar ?? colors.background,
            borderRightColor: colors.border,
            borderRightWidth: isSidebarVisible ? 1 : 0,
            overflow: 'hidden',
          },
          overlayColor: 'transparent',
          drawerActiveTintColor: colors.foreground,
          drawerInactiveTintColor: colors.foreground,
          drawerActiveBackgroundColor: colors.accent,
          drawerInactiveBackgroundColor: 'transparent',
        }}
      >
        {visibleScreens.map((screen) => (
          <Drawer.Screen
            key={screen.route}
            name={screen.route}
            options={{
              drawerLabel: screen.label,
              title: screen.label,
              drawerIcon: screen.icon,
              // @ts-ignore
              roles: screen.roles,
            }}
          />
        ))}

        <Drawer.Screen
          name='more'
          options={{
            drawerItemStyle: { display: 'none' },
          }}
        />

        {hiddenScreens.map((screen) => (
          <Drawer.Screen
            key={screen.route}
            name={screen.route}
            options={{
              drawerItemStyle: { display: 'none' },
            }}
          />
        ))}
      </Drawer>
    </HeaderProvider>
  );
}

const styles = StyleSheet.create({
  showMenuButton: {
    marginLeft: 12,
    width: 40,
    height: 40,
    alignItems: 'center',
    justifyContent: 'center',
  },
});

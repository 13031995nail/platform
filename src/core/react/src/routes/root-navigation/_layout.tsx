import { DynamicHeader, HeaderSystemControls } from '@tmobis/core/components/header';
import { HeaderProvider } from '@tmobis/core/components/header/context';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import { useModules } from '@tmobis/core/modules/provider';
import { filterScreensByRoles } from '@tmobis/core/routes/filter-screens-by-roles';
import { Tabs } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import React, { useMemo } from 'react';

const hiddenTabOptions = {
  href: null,
};

/**
 * Корневой layout мобилки: primary-табы + «Ещё», остальные экраны скрыты из tab bar.
 */
export default function MobileTabsLayout() {
  const { compositionRoot } = useModules();
  const { primary, screens } = compositionRoot.getNavigationPlan();
  const headerActions = useMemo(
    () => compositionRoot.getHeaderActions(),
    [compositionRoot]
  );

  const visiblePrimary = useMemo(() => filterScreensByRoles(primary), [primary]);
  const visiblePrimaryRoutes = useMemo(
    () => new Set(visiblePrimary.map((screen) => screen.route)),
    [visiblePrimary]
  );

  const hiddenNavRoutes = useMemo(
    () =>
      screens
        .map((screen) => screen.route)
        .filter((route) => !visiblePrimaryRoutes.has(route)),
    [screens, visiblePrimaryRoutes]
  );
  const colors = useGluestackColors();

  return (
    <HeaderProvider>
      <Tabs
        screenOptions={{
          headerStyle: {
            backgroundColor: colors.background,
            borderBottomColor: colors.border,
            borderBottomWidth: 1,
            elevation: 0,
            shadowOpacity: 0,
          },
          headerTitle: () => <DynamicHeader />,
          headerRight: () => (
            <HStack className='items-center'>
              {headerActions}
              <HeaderSystemControls />
            </HStack>
          ),
        }}
      >
        {visiblePrimary.map((screen) => (
          <Tabs.Screen
            key={screen.route}
            name={screen.route}
            options={{
              title: screen.label,
              tabBarIcon: screen.icon
                ? ({ color, size, focused }) => screen.icon!({ color, size, focused })
                : undefined,
            }}
          />
        ))}

        <Tabs.Screen
          name='more'
          options={{
            title: 'Ещё',
            tabBarIcon: ({ color, size }) => (
              <Ionicons name='menu' size={size} color={color} />
            ),
          }}
        />

        {hiddenNavRoutes.map((route) => (
          <Tabs.Screen
            key={route}
            name={route}
            options={hiddenTabOptions}
          />
        ))}
      </Tabs>
    </HeaderProvider>
  );
}

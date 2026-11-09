import ModuleBase, {type ModuleLifecycleContext} from './module';
import type { RequireContext } from 'expo-router';
import React from "react";
import { ApiClientProvider } from '@tmobis/core/api/api-client-context';
import QueryProvider from "@tmobis/core/api/queryProvider";
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { DarkTheme, DefaultTheme, ThemeProvider } from '@react-navigation/native';
import { GluestackUIProvider } from '@tmobis/core/gluestack-ui-provider';
import {
  BottomSheetModalProvider
} from '@tmobis/core/components/modal/provider';
import type ScreenData from "@tmobis/core/routes/screen-data";
import ErrorBoundary from '@tmobis/core/components/error-boundary';
import { Ionicons } from '@expo/vector-icons';
import {
  AppPreferencesProvider,
  useAppPreferences,
} from '@tmobis/core/settings/preferences-provider';
import { getCoreSettingsItems } from '@tmobis/core/settings/core-items';
import { settingsLocales } from '@tmobis/core/settings/locales';
import type { SettingsItem } from '@tmobis/core/settings/types';
import type { Resource } from 'i18next';

function CoreThemeShell({ children }: { children: React.ReactNode }) {
  const { resolvedTheme } = useAppPreferences();
  const activeTheme = resolvedTheme === 'dark' ? DarkTheme : DefaultTheme;

  return (
    <GluestackUIProvider mode={resolvedTheme}>
      <BottomSheetModalProvider>
        <ThemeProvider value={activeTheme}>
          {children}
        </ThemeProvider>
      </BottomSheetModalProvider>
    </GluestackUIProvider>
  );
}

/**
 * React модуль ядра Платформы Т-Мобис.
 * Добавляет маршруты Expo Router, экраны навигации, пункты настроек и React provider.
 * API-провайдер получает клиента от CompositionRoot.
 */
export default class CoreModule extends ModuleBase {
  locales?: Resource | undefined = settingsLocales;

  getRouteImports(): RequireContext[] {
    // @ts-ignore — Metro require.context
    return [require.context('../routes/root-navigation'), require.context('../routes/core')];
  }

  getScreens(): ScreenData[] {
    return [
      {
        route: 'desktop',
        label: 'Рабочий стол',
        group: 'Общее',
        primary: true,
        priority: 0,
        icon: ({ size, color }) => (
          <Ionicons name="grid-outline" size={size} color={color} />
        ),
      },
      {
        route: 'settings',
        label: 'Настройки',
        hideInNav: true,
      },
    ];
  }

  getSettingsItems(): SettingsItem[] {
    return getCoreSettingsItems();
  }
  
  wrapProvider(children: React.ReactNode, {apiClient}: ModuleLifecycleContext): React.ReactNode {
    return (
      <ApiClientProvider client={apiClient}>
        <QueryProvider>
          <GestureHandlerRootView style={{ flex: 1 }}>
            <ErrorBoundary>
              <AppPreferencesProvider>
                <CoreThemeShell>{children}</CoreThemeShell>
              </AppPreferencesProvider>
            </ErrorBoundary>
          </GestureHandlerRootView>
        </QueryProvider>
      </ApiClientProvider>
    );
  }
}

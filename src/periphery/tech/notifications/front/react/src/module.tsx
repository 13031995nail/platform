import { Ionicons } from '@expo/vector-icons';
import ModuleBase from '@tmobis/core/modules/module';
import ScreenData from '@tmobis/core/routes/screen-data';
import type { RequireContext } from 'expo-router';
import type { ReactNode } from 'react';
import NotificationBell from './components/notification-bell';

type RouteContextRequire = typeof require & {
  context(directory: './routes'): RequireContext;
};

const getRouteContext = () =>
  (require as RouteContextRequire).context('./routes');

/**
 * React модуль оповещений.
 * Добавляет маршруты Expo Router, экраны навигации и действия заголовка.
 */
export default class NotificationModule extends ModuleBase {
  getHeaderActions(): ReactNode[] {
    return [<NotificationBell key="notification-bell" />];
  }

  getRouteImports(): RequireContext[] {
    return [getRouteContext()];
  }

  getScreens(): ScreenData[] {
    return [{
      route: 'notifications',
      label: 'Уведомления',
      group: 'Система',
      icon: ({ size, color }) => (
        <Ionicons name="notifications-outline" size={size} color={color} />
      ),
    }];
  }
}

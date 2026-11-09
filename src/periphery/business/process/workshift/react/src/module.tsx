import ModuleBase from '@tmobis/core/modules/module';
import ScreenData from "@tmobis/core/routes/screen-data";
import { Ionicons } from '@expo/vector-icons';
import {RequireContext} from "expo-router";
import { navigateToTodayWorkShift } from './navigation/navigate-to-today-work-shift';
import { createMessageApi, type MessageApi } from '@tmobis/core/api/services/message-api';
import type { ApiClient } from '@tmobis/core/api/create-api-client';

/**
 * React бизнес-модуль сущности рабочих смен.
 * Добавляет маршруты Expo Router, экраны навигации и пункты drawer.
 */
export default class WorkShiftModule extends ModuleBase {
  private messageApi?: MessageApi;

  configureApiClient(client: ApiClient) {
    this.messageApi = createMessageApi(client);
  }

  getRouteImports(): RequireContext[] {
    // @ts-ignore
    return [require.context('./routes')];
  }
  
  getScreens(): ScreenData[] {
    return [{
      route: 'work-shifts',
      label: 'Смены',
      group: 'Работа',
      primary: true,
      priority: 1,
      icon: ({ size, color }) => (
        <Ionicons name="calendar-outline" size={size} color={color} />
      )
    }];
  }

  getDrawerItems() {
    const messageApi = this.messageApi;
    if (!messageApi) {
      throw new Error('WorkShiftModule: API client is not configured');
    }
    return [{
      group: 'Работа',
      items: [{
        label: 'Мои задачи',
        icon: ({ size, color }) => (
          <Ionicons name="clipboard-outline" size={size} color={color} />
        ),
        onPress: () => navigateToTodayWorkShift(messageApi),
      }],
    }];
  }
};

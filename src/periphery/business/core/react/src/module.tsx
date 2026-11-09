import { RequireContext } from 'expo-router';
import ScreenData from '@tmobis/core/routes/screen-data';
import { Ionicons } from '@expo/vector-icons';
import React from 'react';
import ModuleBase from '@tmobis/core/modules/module';

/**
 * React бизнес-модуль ядра бизнес-логики Платформы Т-Мобис.
 * Добавляет маршруты Expo Router и экраны навигации.
 */
export default class BusinessCoreModule extends ModuleBase {
  getRouteImports(): RequireContext[] {
    // @ts-ignore
    return [require.context('./routes')];
  }

  getScreens(): ScreenData[] {
    return [{
      route: 'users',
      label: 'Пользователи',
      group: 'Администрирование',
      roles: ['admin'],
      icon: ({ size, color }) => (
        <Ionicons name="people-outline" size={size} color={color} />
      )
    }, {
      route: 'employees',
      label: 'Сотрудники',
      group: 'Администрирование',
      roles: ['admin'],
      icon: ({ size, color }) => (
        <Ionicons name="person-outline" size={size} color={color} />
      )
    }];
  }
}

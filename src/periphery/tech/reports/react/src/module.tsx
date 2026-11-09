import {RequireContext, router} from "expo-router";
import ScreenData from "@tmobis/core/routes/screen-data";
import { Ionicons } from '@expo/vector-icons';
import React from "react";
import ModuleBase from "@tmobis/core/modules/module";
import {DrawerGroupItemsConfig} from "@tmobis/core/components/drawer/drawer-group";

/**
 * React модуль подсистемы отчетов и печатных форм.
 */
export default class ReportsModule extends ModuleBase {
  getRouteImports(): RequireContext[] {
    // @ts-ignore
    return [require.context('./routes')];
  }

  getDrawerItems(): DrawerGroupItemsConfig[] {
    return [{
      group: 'Управление',
      items: [{
        label: 'Пример ПФ',
        icon: ({ size, color }) => (
          <Ionicons name="clipboard-outline" size={size} color={color} />
        ),
        onPress: () => {
          router.navigate({ pathname: '/reports/view', params: { template: 'RedinessAct', p_pk: '6f3089fd-10d6-45e8-8cab-0591cd48da70' } });
        },
      }],
    }];
  }
  
  getScreens(): ScreenData[] {
    return [{
      route: 'reports',
      label: 'Отчеты',
      group: 'Управление',
      icon: ({ size, color }) => (
        <Ionicons name="bar-chart-outline" size={size} color={color} />
      )
    }];
  }
};
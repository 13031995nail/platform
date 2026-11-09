import { Resource } from "i18next";
import type { ApiClient } from '../api/create-api-client';
import { RequireContext } from "expo-router";
import ScreenData from "@tmobis/core/routes/screen-data";
import FieldExtension from "@tmobis/core/components/entity-edit/field-extension";
import { DrawerGroupItemsConfig } from "@tmobis/core/components/drawer/drawer-group";
import type { ListFormExtension } from "@tmobis/core/modules/list-form-extension";
import type {MessageProducer} from '@tmobis/core/messages/dispatcher';
import type {ReactNode} from 'react';
import type {
  DesktopItem,
  DesktopLayoutConfig,
} from '@tmobis/core/desktop/types';
import type { SettingsItem } from '@tmobis/core/settings/types';

export interface ModuleLifecycleContext {
  modules: readonly ModuleBase[];
  apiClient: ApiClient;
}

/**
 * Базовый контракт React-модулей Платформы Т-Мобис.
 * Определяет hooks для регистрации маршрутов, экранов, расширений, providers и жизненного цикла.
 */
export default abstract class ModuleBase {
  locales?: Resource | undefined;

  getRouteImports?(): RequireContext[];

  getScreens?(): ScreenData[];

  getDrawerItems?(): DrawerGroupItemsConfig[];

  getFieldExtensions?(): FieldExtension[];
  
  initStatic?(): void;

  /** Конфигурация транспорта после создания CompositionRoot. */
  configureApiClient?(client: ApiClient): void;

  getListFormExtensions?(): readonly ListFormExtension[];

  getMessageProducers?(): MessageProducer[];

  getHeaderActions?(): ReactNode[];

  /**
   * Вклад плиток на рабочий стол (игнорируется, если задан override).
   */
  getDesktopItems?(): DesktopItem[];

  /**
   * Полное переопределение перечня плиток рабочего стола.
   * Используется первый модуль, вернувший значение.
   */
  getDesktopItemsOverride?(): DesktopItem[];

  /**
   * Параметры сетки рабочего стола (колонки, размер иконок, gap).
   * Поля мержатся по порядку модулей: позднее значение поля побеждает.
   */
  getDesktopConfig?(): Partial<DesktopLayoutConfig>;

  /**
   * Вклад пунктов экрана настроек (грид акцентных кнопок и список кнопка + описание).
   */
  getSettingsItems?(): SettingsItem[];

  wrapProvider?(children: ReactNode, context: ModuleLifecycleContext): ReactNode;

  initialize?(context: ModuleLifecycleContext): void | Promise<void>;

  dispose?(): void | Promise<void>;
}

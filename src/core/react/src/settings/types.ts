import type { ComponentType } from 'react';

/**
 * Вариант пункта настроек: акцентная кнопка в гриде или строка кнопка + описание.
 */
export type SettingsItemVariant = 'action' | 'row';

export type SettingsItem = {
  id: string;
  caption: string;
  /** Пояснение или текущее значение (для variant=row). */
  description?: string;
  /** Группа (модуль / подсистема). */
  group: string;
  groupOrder?: number;
  order?: number;
  variant: SettingsItemVariant;
  roles?: string[];
  href?: string;
  onPress?: () => void | Promise<void>;
  /**
   * Кастомный рендер строки (variant=row) вместо кнопки с caption.
   * Для интерактивных пунктов ядра (язык, тема и т.д.).
   */
  component?: ComponentType<{ item: SettingsItem }>;
};

export type SettingsSection = {
  group: string;
  groupOrder: number;
  items: SettingsItem[];
};

export type SettingsGroupSection = {
  group: string;
  groupOrder: number;
  actions: SettingsItem[];
  rows: SettingsItem[];
};

export const CORE_SETTINGS_GROUP = 'Общее';
export const CORE_SETTINGS_GROUP_ORDER = 1000;

export const SETTINGS_LOCALE_KEY = 'tmobis.settings.locale';
export const SETTINGS_THEME_KEY = 'tmobis.settings.theme';

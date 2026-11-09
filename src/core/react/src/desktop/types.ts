import type { ReactNode } from 'react';

/**
 * Плитка рабочего стола (иконка-кнопка).
 */
export type DesktopItem = {
  id: string;
  caption: string;
  title?: string;
  /** Путь expo-router, например '/work-shifts'. */
  href: string;
  icon: (props: { color: string; size: number }) => ReactNode;
  roles?: string[];
  order?: number;
  /** Цвет иконки; по умолчанию brand primary. */
  color?: string;
};

/**
 * Параметры сетки рабочего стола.
 */
export type DesktopLayoutConfig = {
  columns: number;
  iconSize: number;
  gap: number;
};

export const DEFAULT_DESKTOP_LAYOUT: DesktopLayoutConfig = {
  columns: 5,
  iconSize: 64,
  gap: 20,
};

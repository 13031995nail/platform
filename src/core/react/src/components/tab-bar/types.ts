import type { ReactNode, ElementType } from 'react';

export type TabBarItem = {
  /** Уникальный идентификатор вкладки (как `id` в Ember `tmobis-tabs`) */
  id: string;
  label: string;
  icon?: ElementType;
  content: ReactNode;
  disabled?: boolean;
};

// TODO: Перевести на children
export type TabBarProps = {
  tabs: TabBarItem[];
  /** Контролируемая активная вкладка */
  value?: string;
  /** Начальная вкладка (неконтролируемый режим) */
  defaultValue?: string;
  onValueChange?: (id: string) => void;
  variant?: 'filled' | 'underlined';
  orientation?: 'horizontal' | 'vertical';
  /**
   * Монтировать контент вкладки только после первого открытия.
   * Для общей модели формы между вкладками оставляйте `false` (по умолчанию).
   */
  lazy?: boolean;
  disabled?: boolean;
  /**
   * Свайп между панелями.
   * В `@tmobis/core/components/ui/tabs` (Gluestack) свайпа нет — только переключение по нажатию.
   * Для свайпа потребуется `react-native-tab-view` (отдельная зависимость); флаг пока не реализован.
   */
  swipeEnabled?: boolean;
};

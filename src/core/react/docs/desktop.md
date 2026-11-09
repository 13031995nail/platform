# Рабочий стол

Экран `/desktop` («Рабочий стол») — сетка иконок-кнопок, аналог Ember `#/desktop`.

Роут и пункт навигации регистрирует `CoreModule`. Список плиток и параметры сетки собирает `CompositionRoot`.

## Плитки из модуля

```tsx
import type { DesktopItem } from '@tmobis/core/desktop/types';
import { Ionicons } from '@expo/vector-icons';

getDesktopItems(): DesktopItem[] {
  return [{
    id: 'reports',
    caption: 'Отчёты',
    href: '/reports',
    order: 10,
    icon: ({ size, color }) => (
      <Ionicons name="document-text-outline" size={size} color={color} />
    ),
  }];
}
```

Вклады всех модулей объединяются и сортируются по `order`. Роли (`roles`) фильтруются так же, как у экранов.

Цвет иконки по умолчанию — brand `primary`. Можно задать `color` на плитке.

## Полное переопределение на уровне приложения

Если модуль (обычно `AppModule`) реализует `getDesktopItemsOverride()`, его список **заменяет** все вклады модулей. Используется первый модуль, вернувший значение.

```tsx
getDesktopItemsOverride(): DesktopItem[] {
  return [/* полный перечень приложения */];
}
```

## Размер сетки и иконок

```tsx
import type { DesktopLayoutConfig } from '@tmobis/core/desktop/types';

getDesktopConfig(): Partial<DesktopLayoutConfig> {
  return {
    columns: 3,   // default 5
    iconSize: 80, // default 64
    gap: 24,      // default 20
  };
}
```

Поля мержатся по порядку модулей: позднее значение поля побеждает. Defaults — `DEFAULT_DESKTOP_LAYOUT` в `@tmobis/core/desktop/types`.

## Лендинг

В шаблоне `expo-app` роут `index` редиректит на `/desktop`, а `desktop` — primary-таб с `priority: 0`.

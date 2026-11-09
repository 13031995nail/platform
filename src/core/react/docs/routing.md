## Как создать новый роут (страницу)
В системе модулей роуты объявляются в каждом модуле отдельно через функцию `getScreens()`.

### Создание роута в модуле
1. Создать файл страницы в папке `routes/` модуля (например, `имяРоута.tsx` или папка `имяГруппы`)
2. В объекте модуля реализовать функцию `getScreens()`:

Пример:
```tsx
import ScreenData from "@tmobis/core/routes/screen-data";
import { Ionicons } from "@expo/vector-icons";

export default class MyModule implements IModule {
  getScreens?(): ScreenData[] {
    return [
      {
        route: "путьРоута",
        label: "имяРоута",
        group: "имяГруппы",
        primary: true,   // показывать в bottom tabs на мобиле
        priority: 1,     // порядок primary-таба
        icon: ({ color, size, focused }) => (
          <Ionicons name="home" size={size} color={color} />
        ),
        roles: ["admin"],
      },
    ];
  }
}
```

Поля `ScreenData` для навигации:
- `primary` / `priority` — попадание в нижние табы (мобила), иначе экран идёт во вкладку **«Ещё»**
- `group` — секция в drawer (веб) и в «Ещё» (мобила)
- `hideInNav` — скрыть из UI-навигации (служебные роуты)
- `roles` — фильтрация по ролям пользователя

## Корневая навигация (RootNavigation)

Корневые layout’ы живут в ядре платформы — в `CoreModule` (`@tmobis/core/modules/core-module`):

| Платформа | Файл в ядре | Навигатор |
|-----------|-------------|-----------|
| **Web** | `routes/root-navigation/_layout.web.tsx` | **Drawer** (сайдбар со всеми разделами по группам) |
| **Mobile** | `routes/root-navigation/_layout.tsx` | **Tabs** (до 4 primary + вкладка «Ещё») |
| **Mobile** | `routes/root-navigation/more.tsx` | Экран дополнительных разделов |

Подключение в приложении — через `getRouteImports()` и merge с `prefixContext(..., '(tabs)')` (как у entity-роутов). В шаблоне `expo-app` достаточно зарегистрировать `new CoreModule()` в массиве модулей; локальные `(tabs)/_layout*` / `more` не дублируются.

Защита маршрутов — не в layout ядра, а через `KeycloakModule.wrapProvider` (`ProtectedRoute`).

Экраны верхнего уровня берутся из `compositionRoot.getNavigationPlan()` (модули).

Лендинг по умолчанию — **Рабочий стол** (`/desktop`). Подробнее: [desktop.md](./desktop.md).

Экран **Настройки** (`/settings`) скрыт из навигации (`hideInNav`) и открывается из хедера. Подробнее: [settings.md](./settings.md).

### Почему не все формы в табах

Bottom tabs рассчитаны на **3–5** равноправных разделов. Каталог сущностей (как в старых сайдбарах) в табы не кладётся:

- **Tabs** — только частые primary-разделы (`primary: true`)
- **«Ещё»** (мобила) — остальные экраны + кастомные drawer-пункты, сгруппированные как в сайдбаре
- **Drawer** (веб) — полный каталог с группами
- Служебные роуты (`hideInNav: true`, например oauthredirect) скрыты из навигации

```
Web:     Drawer (группы)
         └── Stack сущности: list → new/[id] → …

Mobile:  Tabs
         ├── primary (Рабочий стол, Смены, …)
         ├── Ещё → secondary-список
         └── Stack сущности (те же URL, tab скрыт через href: null)
```

### Навигация внутри сущности

Для сущности используется **Stack** в папке роута:

```
routes/users/
  _layout.tsx   # Stack, headerShown: false
  index.tsx     # списковая форма
  new.tsx       # создание
  [id].tsx      # редактирование
```

Типичный поток: список → create/edit (поверх стека с возвратом назад).

- **Drawer / Tabs** — плоский корень (разделы приложения).
- **Stack** — иерархия внутри сущности (list → edit → detail).

Пример Stack:

```tsx
import { Stack } from 'expo-router';

export default function UsersLayout() {
  return <Stack screenOptions={{ headerShown: false }} />;
}
```

Переход со списка на редактирование — через `EntityListForm` (`router.navigate(\`/${route}/${id}\`)`) или кнопку создания (`/${route}/new`).

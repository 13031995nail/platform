# TmobisDropdown / TmobisSimpleDropdown

Выпадающий список: single/multi, enum или массив сущностей, локальный и серверный поиск.

* `TmobisDropdown` — для форм EntityEdit (`fieldName`, `label`, валидация).
* `TmobisSimpleDropdown` — автономное использование (например, внутри групэдита).

## Поиск

* `searchable` — включает поле поиска (локальная фильтрация по label).
* `onSearch` — серверный/внешний поиск; при наличии локальный filter отключается. Вызов debounced.
* `searchPlacement` — `'trigger'` (по умолчанию) или `'panel'` (поле поиска внутри выпадающего меню).
* `minSearchLength` — минимальная длина строки для запуска поиска (по умолчанию `0`).
* `searchDebounceMs` — задержка debounce для `onSearch` (по умолчанию `1000`).
* `searchPlaceholder` — placeholder поля поиска в панели.

`TmobisDropdownLookup` использует `searchPlacement="panel"`, `minSearchLength={2}`, `searchDebounceMs={300}` и передаёт `onSearch` в `listData.filter.applyFilterValues` (`getEntities?search=`). Поиск без учёта регистра. `useDropdownLookup` отключает count-запрос (`withCount: false`).

## Пример использования
### На форме EntityEdit
```tsx
import EntityEdit from '@tmobis/core/components/entity-edit';
import { TmobisDropdown } from '@tmobis/core/components/dropdown';

<EntityEdit {...data}>
	<TmobisDropdown
		fieldName="townId"
		label="Город"
		placeholder="Выберите город"
		options={towns}
		getOptionLabel={(town) => town.cityName}
		searchable
	/>
</EntityEdit>
```

### Автономное использование
```tsx
import { TmobisSimpleDropdown } from '@tmobis/core/components/dropdown';

<TmobisSimpleDropdown
	options={{ Active: 'Активен', Blocked: 'Заблокирован' }}
	value={status}
	onChange={setStatus}
/>
```

### Серверный поиск в панели
```tsx
<TmobisSimpleDropdown
	options={entities}
	value={selectedId}
	onChange={setSelectedId}
	getOptionLabel={(entity) => entity.name}
	searchable
	searchPlacement="panel"
	minSearchLength={2}
	searchDebounceMs={300}
	onSearch={(query) => listData.filter.applyFilterValues(
		listData.filter.activeFilter ?? {},
		query ?? undefined
	)}
	loading={listData.isLoading}
/>
```


## Экран ручной проверки (dev)

В шаблоне `src/templates/expo-app` при `__DEV__ === true` и роли
`admin` доступна плитка **«Компоненты»** (маршрут `/component-lab`).

Страница организована как расширяемый каталог со сворачиваемыми разделами:
**«Лукапы»**, **«Дропдауны»**, **«Диагностика»**. Каждый раздел
раскрывается независимо; новые группы компонентов можно добавлять отдельно
от существующих примеров. Тексты сценариев описывают ожидаемое поведение
без персональных обращений.

Экран позволяет проверить:

- single/multi `TmobisDropdownLookup` с имитацией серверного поиска (без API);
- single lookup с одним `valueId`, без `value` / `onChange`;
- single/multi `TmobisSimpleDropdown` для массива и enum;
- реальный `TmobisDropdownLookup` со справочником `TownE`;
- текущие ID/объекты и последние вызовы `onChange`, `onChangeId`, `onSearch`.

Проверка регрессии: выбрать «Москва», отфильтровать её поиском «Томск»;
в основном поле должна продолжать отображаться «Москва». В multi-режиме
выбрать объекты в разных поисковых выдачах и убедиться, что прежние
не исчезли. Реальное число HTTP-запросов проверять на примере `TownE`
через Network DevTools, поскольку имитация API не вызывает.

import type { IIconComponentType } from "@gluestack-ui/core/lib/esm/icon/creator/createIcon";
import type { ListFormType } from "@tmobis/core/modules/list-form-extension";
import type { ReactNode } from "react";
import type { SvgProps } from "react-native-svg";
import type { IEntity } from "../../models/entity";
import type { EntityListSystemMenuItem } from "./toolbar-shared";
import type { EntityId, EntityListData } from "./use-entity-list";

export type { EntityId };

/**
 * Кнопки контекстного меню
 */
export interface EntityListViewButton<TEntity extends IEntity> {
	/**
	 * Название кнопки
	 */
	name: string;

	/**
	 * Обработчик кнопки
	 */
	action?: (rowData: TEntity) => void;

	/**
	 * Видимость кнопки
	 */
	isVisible: boolean | ((rowData: TEntity) => boolean);

	/**
	 * Иконка кнопки
	 */
	icon: IIconComponentType<SvgProps>;

	/**
	 * Признак стандартного действия удаления.
	 */
	isDeleteAction?: boolean;
}

/**
 * Настройка отображения строки таблицы и мобильной карточки.
 */
export interface EntityListRowConfig {
	/** Оформление строки и карточки. */
	className?: string;

	/** Запрет выбора записи. */
	canBeSelected?: boolean;

	/** Запрет стандартного действия удаления в меню строки. */
	canBeDeleted?: boolean;

	/** Содержимое заголовка мобильной карточки. */
	mobileCardTitle?: ReactNode;

	/** Поле, используемое как заголовок мобильной карточки. */
	mobileCardTitleColumn?: string;

	/** Поля, показываемые в свёрнутой мобильной карточке. */
	mobileCardColumnsToShow?: readonly string[];
}

/**
 * Описание колонки списка сущностей
 */
export interface EntityListViewColumn<TEntity extends IEntity = IEntity> {
	/**
	 * Название поля сущности
	 */
	column?: string;

	/**
	 * Отображаемое имя колонки
	 */
	title?: string;

	/**
	 * Функция рендеринга ячейки
	 * @param rowData - данные строки
	 * @returns ReactNode
	 */
	renderComponent?: (rowData: TEntity) => ReactNode;
}

export interface EntityListProps<TEntity extends IEntity> {
	/**
	 * Тип списковой формы для применения модульных расширений.
	 */
	listFormType?: ListFormType;

	/**
	 * Адрес формы редактирования сущности.
	 */
	editRoute?: string;

	/**
	 * Дополнительные дочерние компоненты
	 */
	children?: ReactNode;

	/**
	 * Фильтры системной строки (дропдауны)
	 */
	toolbarFilters?: ReactNode;

	/**
	 * Пункты системного трея
	 */
	systemMenuItems?: EntityListSystemMenuItem[];

	/**
	 * Данные из хука {@link useEntityList}
	 */
	listData: EntityListData<TEntity>;

	/**
	 * Описание колонок списка сущностей
	 */
	columns: EntityListViewColumn<TEntity>[];

	/**
	 * Событие нажатия на строку списка
	 */
	onRowClick?: (rowData: TEntity) => void;

	/**
	 * Событие создания новой записи
	 */
	onAdd?: () => void;

	/**
	 * Используется ли фильтрация (кнопка "Применить" у текстовых фильтров)
	 */
	hasFilter?: boolean;

	/**
	 * Используется ли поиск
	 */
	hasSearch?: boolean;

	/**
	 * Используется ли выделение строк
	 */
	hasSelection?: boolean;

	/**
	 * Контролируемые ID выбранных записей (сверка по id).
	 */
	selectedIds?: EntityId[];

	/**
	 * Изменение контролируемого выбора.
	 * `entities` записи с текущей страницы, попавшие в `ids`.
	 */
	onSelectedIdsChange?: (ids: EntityId[], entities: TEntity[]) => void;

	/**
	 * Показ кнопки удаления в системной строке
	 */
	showDeleteButton?: boolean;

	/**
	 * Показ кнопки добавления в системной строке
	 */
	showAddButton?: boolean;

	/**
	 * Показ кнопки обновления в системной строке
	 */
	showRefreshButton?: boolean;

	/**
	 * Показ системной строки
	 */
	showToolbar?: boolean;

	/**
	 * Массив кнопок контекстного меню
	 */
	buttons?: EntityListViewButton<TEntity>[];

	/**
	 * Видимость кнопки удаления
	 */
	showDeleteMenuItemInRow?: boolean;

	/**
	 * Видимость кнопки редактирования
	 */
	showEditMenuItemInRow?: boolean;

	/**
	 * Настройка отображения отдельной строки.
	 */
	getRowConfig?: (rowData: TEntity) => EntityListRowConfig;

	/**
	 * Версия внешних зависимостей getRowConfig (например, прав доступа).
	 * Изменение значения заставляет пересчитать кеш настроек строк.
	 */
	rowConfigVersion?: string | number;

	/**
	 * Показ скрытых полей карточки по нажатию на стрелку.
	 */
	mobileCardExpandable?: boolean;
}

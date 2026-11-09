import { ListFilterValues } from "@tmobis/core/api/filters";
import type { IEntity } from "@tmobis/core/models/entity";
import { useCallback, useState } from "react";
import {
	useDeleteEntities,
	useDeleteEntity,
	useGetEntities
} from "../../api/hooks/useEntityApi";
import {
	FormStateController,
	useFormState,
	useFormStateLoading,
} from "../form-state";
import useListFilter, { ListFilterData } from "./use-entity-list-filter";
import useEntityListPaging, { EntityListPagingData } from "./use-entity-list-paging";
import useEntityListSorting from "./use-entity-list-sorting";
import { setEntityIdSelected, toggleSelectedIds } from "./entity-selection-utils";

/**
 * Идентификатор сущности для выбора строк.
 */
export type EntityId = string;

export interface UseEntityListProps {
	/**
	 * Тип сущности (используется для API запросов)
	 */
	entityType: string;

	/**
	 * Представление сущности
	 */
	viewName?: string;

	/**
	 * Перечень фильтров
	 */
	filterFields?: string[];

	/**
	 * Хранение параметров списка в URL
	 */
	useUrlParams?: boolean;

	/**
	 * Переопределенное стандартное количество записей на страницу
	 */
	defaultPerPage?: number;

	/**
	 * Переопределенные значения фильтрации
	 */
	defaultFilter?: ListFilterValues;

	/** Запрашивать общее количество сущностей в ответе списка. */
	withCount?: boolean;
}

/**
 * Данные, возвращаемые хуком {@link useEntityList}
 */
export interface EntityListData<TEntity extends IEntity> {
	/**
	 * Тип сущности.
	 */
	entityType: string;

	/**
	 * Представление сущности.
	 */
	viewName?: string;

	/**
	 * Список сущностей
	 */
	data: TEntity[] | undefined;

	/**
	 * Состояние формы списка
	 */
	formState: FormStateController;

	/**
	 * Статус загрузки данных
	 */
	isLoading: boolean;

	/**
	 * Состояние и функции фильтрации
	 */
	filter: ListFilterData;

	/**
	 * Удаление записи списка
	 */
	deleteEntity: (id?: string) => void;

	/**
	 * Удаление выбранных записей списка.
	 * Переданные ID используются вместо внутреннего выбора (controlled-режим).
	 */
	deleteSelected: (ids?: readonly EntityId[]) => Promise<void>;

	/**
	 * Обновление данных списка
	 */
	refresh: () => Promise<void>;

	/**
	 * Состояние и функции пагинации
	 */
	paging: EntityListPagingData;

	/**
	 * Состояние и функции сортировки
	 */
	sorting: {
		/**
		 * Текущее поле сортировки
		 */
		value: string | undefined;

		/**
		 * Переключение сортировки по полю
		 */
		toggle: (column: string) => void;
	};

	/**
	 * Состояние и функции выбора строк
	 */
	selection: {
		/**
		 * Идентификаторы выбранных сущностей
		 */
		selected: Set<EntityId>;

		/**
		 * Выбор сущности по идентификатору
		 */
		setSelected: (entity: TEntity, isSelected: boolean) => void;

		/**
		 * Выбор всех переданных (или текущей страницы) сущностей одним обновлением Set
		 */
		setSelectAll: (entities?: readonly TEntity[]) => void;
	};
}

/**
 * Хук для управления состоянием списка сущностей и получения данных.
 * Объединение логики пагинации, сортировки, фильтрации и выбора строк.
 *
 * @typeParam TEntity - Тип сущности
 * @param props - Параметры {@link UseEntityListProps}
 * @returns Объект с данными списка {@link EntityListData}
 *
 * @example
 * ```ts
 * import { IUser, IUserClassName } from "@/models/iuser";
 * 
 * const listData = useEntityList<IUser>({
 *   entityType: IUserClassName,
 *   viewName: 'UserE'
 * });
 * ```
 */
export default function useEntityList<TEntity extends IEntity>({
	entityType, viewName, filterFields,
	useUrlParams = true, defaultPerPage, defaultFilter, withCount = true
}: UseEntityListProps): EntityListData<TEntity> {
	const [selected, setSelected] = useState<Set<EntityId>>(new Set());
	const formState = useFormState();

	const {
		page,
		setPage,
		perPage,
		setPerPage,
	} = useEntityListPaging({ useUrlParams, defaultPerPage });

	const onFilterApply = useCallback(() => {
		setPage(1);
		setSelected(new Set());
	}, [setPage]);

	const { sorting, toggleSorting } = useEntityListSorting({
		onSortingChanged: onFilterApply,
		useUrlParams,
	});

	const filter = useListFilter({
		onFilterApply, filterFields, useUrlParams,
		defaultFilter
	});
	const { activeSearch, activeFilter } = filter;

	const handleNotFound = () => {
		alert('НЕ НАЙДЕНА СУЩНОСТЬ!');
	};

	// Получение списка и количества записей через PagedResult.
	const { data: pagedResponse, isLoading, refetch: refetchList } = useGetEntities<TEntity>({
		entityType,
		viewName,
		onNotFound: handleNotFound,
		page,
		perPage,
		sorting,
		filter: activeFilter,
		search: activeSearch,
		includeTotalCount: withCount
	});

	const data = pagedResponse?.items;
	const totalCount = withCount ? pagedResponse?.totalCount : undefined;

	const isDataLoading = isLoading;
	useFormStateLoading(formState, isDataLoading);

	const canGoNext = !withCount
		? (data?.length ?? 0) >= perPage
		: totalCount === undefined || totalCount === null
			? true
			: page * perPage < totalCount;

	const { deleteEntity: deleteEntityInternal } = useDeleteEntity({
		entityType,
	});
	const { deleteEntities: deleteEntitiesInternal } = useDeleteEntities({
		entityType,
	});

	const setSelectedState = useCallback((entity: TEntity, isSelected: boolean) => {
		const id = entity.id;
		if (!id) {
			return;
		}
		setSelected(prevSelected => setEntityIdSelected(prevSelected, id, isSelected));
	}, []);

	const setSelectAll = useCallback((entities?: readonly TEntity[]) => {
		const pageData = (data ?? []) as TEntity[];
		const targets = entities ?? pageData;
		const ids = targets
			.map(entity => entity.id)
			.filter((id): id is EntityId => Boolean(id));
		setSelected(prevSelected => toggleSelectedIds(prevSelected, ids));
	}, [data]);

	const deleteEntity = useCallback(async (id?: string) => {
		if (id !== undefined && id !== '') {
			await deleteEntityInternal(id);
		}
	}, [deleteEntityInternal]);

	const deleteSelected = useCallback(async (explicitIds?: readonly EntityId[]) => {
		const ids = [...new Set(explicitIds ?? selected)].filter(Boolean);
		if (ids.length === 0) {
			return;
		}
		await deleteEntitiesInternal(ids);
		setSelected(new Set());
	}, [deleteEntitiesInternal, selected]);

	const refresh = useCallback(async () => {
		await refetchList();
	}, [refetchList]);

	return {
		entityType,
		viewName,
		data,
		formState,
		isLoading: !formState.isReady,
		filter,
		deleteEntity,
		deleteSelected,
		refresh,
		paging: {
			page,
			setPage,
			perPage,
			setPerPage,
			totalCount,
			canGoNext,
		},
		sorting: {
			value: sorting,
			toggle: toggleSorting,
		},
		selection: {
			selected,
			setSelected: setSelectedState,
			setSelectAll,
		}
	};
}

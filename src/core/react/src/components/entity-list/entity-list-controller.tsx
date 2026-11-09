import { useCallback, useMemo, type ReactNode } from "react";
import type { IEntity } from "../../models/entity";
import { EntityListContext } from "./entity-list-context";
import type {
	EntityListProps,
	EntityListRowConfig,
} from "./entity-list-types";
import { resolveRowConfig } from "./entity-list-row";
import type { EntityId } from "./use-entity-list";
import { useEntityRowActions } from "./use-entity-row-actions";
import { useEntitySelection } from "./use-entity-selection";
import useResolvedListFormColumns from "./use-resolved-list-form-columns";

export type EntityListControllerProps<TEntity extends IEntity> =
	Omit<EntityListProps<TEntity>, "children" | "showToolbar">
	& {
		children: ReactNode;
	};

/**
 * Контроллер списка: колонки, выбор, действия строк и контекст представления.
 */
export function EntityListController<TEntity extends IEntity>({
	columns,
	onRowClick,
	onAdd,
	listData,
	children,
	toolbarFilters,
	systemMenuItems,
	listFormType,
	editRoute,
	hasFilter = false,
	hasSearch = true,
	hasSelection = true,
	selectedIds,
	onSelectedIdsChange,
	showDeleteButton = true,
	showAddButton = true,
	showRefreshButton = true,
	buttons = [],
	showDeleteMenuItemInRow = true,
	showEditMenuItemInRow = true,
	getRowConfig,
	rowConfigVersion,
	mobileCardExpandable = true,
}: EntityListControllerProps<TEntity>) {
	const {
		data,
		deleteEntity,
		selection: {
			selected,
			setSelected,
			setSelectAll,
		},
	} = listData;

	const resolvedColumns = useResolvedListFormColumns({
		columns,
		listFormType,
		entityType: listData.entityType,
		viewName: listData.viewName,
		editRoute,
	});

	const rowConfigsById = useMemo(() => {
		const map = new Map<EntityId, EntityListRowConfig>();
		for (const entity of data ?? []) {
			if (entity.id) {
				map.set(entity.id, resolveRowConfig(entity, getRowConfig));
			}
		}
		return map;
	}, [data, getRowConfig, rowConfigVersion]);

	const resolveRowConfigFor = useCallback((entity: TEntity) => {
		if (entity.id) {
			const cached = rowConfigsById.get(entity.id);
			if (cached) {
				return cached;
			}
		}
		return resolveRowConfig(entity, getRowConfig);
	}, [getRowConfig, rowConfigsById]);

	const {
		isSelectionControlled,
		effectiveSelectedIds,
		isAllSelected,
		selectedCount,
		isEntitySelected,
		setEntitySelected,
		toggleSelectAll,
	} = useEntitySelection({
		data,
		selected,
		setSelected,
		setSelectAll,
		selectedIds,
		onSelectedIdsChange,
		rowConfigsById,
		getRowConfig,
	});

	const deleteSelected = useCallback(async () => {
		if (!isSelectionControlled) {
			await listData.deleteSelected();
			return;
		}
		const ids = [...effectiveSelectedIds];
		if (ids.length === 0) {
			return;
		}
		await listData.deleteSelected(ids);
		onSelectedIdsChange?.([], []);
	}, [effectiveSelectedIds, isSelectionControlled, listData.deleteSelected, onSelectedIdsChange]);

	const { rowButtons, hasRowActions } = useEntityRowActions({
		buttons,
		onRowClick,
		showEditMenuItemInRow,
		showDeleteMenuItemInRow,
		deleteEntity,
	});

	return (
		<EntityListContext.Provider value={{
			listData,
			deleteSelected,
			columns: resolvedColumns,
			rowButtons,
			hasRowActions,
			hasSelection,
			isAllSelected,
			selectedCount,
			isEntitySelected,
			setEntitySelected,
			toggleSelectAll,
			mobileCardExpandable,
			onRowClick,
			resolveRowConfig: resolveRowConfigFor,
			onAdd,
			toolbarFilters,
			systemMenuItems,
			hasFilter,
			hasSearch,
			showDeleteButton,
			showAddButton,
			showRefreshButton,
		}}>
			{children}
		</EntityListContext.Provider>
	);
}

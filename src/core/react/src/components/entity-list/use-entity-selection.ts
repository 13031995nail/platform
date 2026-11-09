import { useCallback, useMemo } from "react";
import type { IEntity } from "../../models/entity";
import type { EntityListRowConfig } from "./entity-list-types";
import type { EntityId } from "./use-entity-list";
import { resolveRowConfig } from "./entity-list-row";
import { getPageEntitiesByIds, getSelectableEntities, setEntityIdSelected, toggleSelectedIds } from "./entity-selection-utils";

export interface UseEntitySelectionProps<TEntity extends IEntity> {
	data: TEntity[] | undefined;
	selected: Set<EntityId>;
	setSelected: (entity: TEntity, isSelected: boolean) => void;
	setSelectAll: (entities?: readonly TEntity[]) => void;
	selectedIds?: EntityId[];
	onSelectedIdsChange?: (ids: EntityId[], entities: TEntity[]) => void;
	rowConfigsById: Map<EntityId, EntityListRowConfig>;
	getRowConfig?: (rowData: TEntity) => EntityListRowConfig;
}

export interface EntitySelection<TEntity extends IEntity> {
	isSelectionControlled: boolean;
	effectiveSelectedIds: ReadonlySet<EntityId>;
	selectableData: TEntity[];
	isAllSelected: boolean;
	selectedCount: number;
	isEntitySelected: (entity: TEntity) => boolean;
	setEntitySelected: (entity: TEntity, isSelected: boolean) => void;
	toggleSelectAll: () => void;
}

/**
 * Выбор строк списка по идентификаторам сущности.
 */
export function useEntitySelection<TEntity extends IEntity>({
	data,
	selected,
	setSelected,
	setSelectAll,
	selectedIds,
	onSelectedIdsChange,
	rowConfigsById,
	getRowConfig,
}: UseEntitySelectionProps<TEntity>): EntitySelection<TEntity> {
	const isSelectionControlled = selectedIds !== undefined
		&& onSelectedIdsChange !== undefined;

	const resolveConfig = useCallback((entity: TEntity) => {
		if (entity.id) {
			const cached = rowConfigsById.get(entity.id);
			if (cached) {
				return cached;
			}
		}
		return resolveRowConfig(entity, getRowConfig);
	}, [getRowConfig, rowConfigsById]);

	const selectableData = useMemo(
		() => getSelectableEntities(data, entity => resolveConfig(entity).canBeSelected !== false),
		[data, resolveConfig]
	);

	const effectiveSelectedIds = useMemo<ReadonlySet<EntityId>>(
		() => isSelectionControlled ? new Set(selectedIds ?? []) : selected,
		[isSelectionControlled, selectedIds, selected]
	);

	const isEntitySelected = useCallback((entity: TEntity) => (
		Boolean(entity.id) && effectiveSelectedIds.has(entity.id!)
	), [effectiveSelectedIds]);

	const isAllSelected = selectableData.length > 0
		&& selectableData.every(entity => effectiveSelectedIds.has(entity.id!));

	const selectedCount = effectiveSelectedIds.size;

	const emitControlledSelection = useCallback((nextIds: EntityId[]) => {
		if (!onSelectedIdsChange) {
			return;
		}
		const nextSet = new Set(nextIds);
		onSelectedIdsChange(nextIds, getPageEntitiesByIds(data, nextSet));
	}, [data, onSelectedIdsChange]);

	const toggleSelectAll = useCallback(() => {
		if (isSelectionControlled) {
			const pageIds = selectableData.map(entity => entity.id!);
			const next = toggleSelectedIds(new Set(effectiveSelectedIds), pageIds);
			if (pageIds.length > 0) {
				emitControlledSelection([...next]);
			}
			return;
		}
		setSelectAll(selectableData);
	}, [emitControlledSelection, effectiveSelectedIds, isSelectionControlled, selectableData, setSelectAll]);

	const setEntitySelected = useCallback((entity: TEntity, isSelected: boolean) => {
		const id = entity.id;
		if (!id) {
			return;
		}
		if (isSelectionControlled) {
			const current = new Set(effectiveSelectedIds);
			const next = setEntityIdSelected(current, id, isSelected);
			if (next !== current) {
				emitControlledSelection([...next]);
			}
			return;
		}
		setSelected(entity, isSelected);
	}, [effectiveSelectedIds, emitControlledSelection, isSelectionControlled, setSelected]);

	return {
		isSelectionControlled,
		effectiveSelectedIds,
		selectableData,
		isAllSelected,
		selectedCount,
		isEntitySelected,
		setEntitySelected,
		toggleSelectAll,
	};
}

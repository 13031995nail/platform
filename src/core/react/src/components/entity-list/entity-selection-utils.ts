import type { IEntity } from "../../models/entity";
import type { EntityId } from "./use-entity-list";

/** Возвращает только записи с ID, доступные для выбора. */
export function getSelectableEntities<TEntity extends IEntity>(
	data: readonly TEntity[] | undefined,
	canSelect: (entity: TEntity) => boolean,
): TEntity[] {
	return data?.filter(entity => Boolean(entity.id) && canSelect(entity)) ?? [];
}

/** Изменяет выбор одной записи без лишнего копирования неизменившегося Set. */
export function setEntityIdSelected(
	selected: Set<EntityId>,
	id: EntityId,
	isSelected: boolean,
): Set<EntityId> {
	if (!id || selected.has(id) === isSelected) {
		return selected;
	}
	const next = new Set(selected);
	if (isSelected) {
		next.add(id);
	} else {
		next.delete(id);
	}
	return next;
}

/** Переключает выбор указанных ID за одно копирование Set. */
export function toggleSelectedIds(
	selected: Set<EntityId>,
	ids: readonly EntityId[],
): Set<EntityId> {
	const uniqueIds = [...new Set(ids.filter(Boolean))];
	if (uniqueIds.length === 0) {
		return selected;
	}
	const allSelected = uniqueIds.every(id => selected.has(id));
	const next = new Set(selected);
	for (const id of uniqueIds) {
		if (allSelected) {
			next.delete(id);
		} else {
			next.add(id);
		}
	}
	return next;
}

/** Находит выбранные записи текущей страницы по идентификаторам. */
export function getPageEntitiesByIds<TEntity extends IEntity>(
	data: readonly TEntity[] | undefined,
	ids: ReadonlySet<EntityId>,
): TEntity[] {
	return data?.filter(entity => Boolean(entity.id) && ids.has(entity.id!)) ?? [];
}

import type { IEntity } from '../../models/entity';

/** Сохраняет выбранные сущности при смене серверной поисковой выдачи. */
export function mergeSelectedEntities<TEntity extends IEntity>(
	previous: TEntity[],
	nextIds: string[],
	pageEntities: TEntity[]
): TEntity[] {
	const previousById = new Map(
		previous
			.filter((entity) => entity.id)
			.map((entity) => [entity.id as string, entity])
	);
	const pageById = new Map(
		pageEntities
			.filter((entity) => entity.id)
			.map((entity) => [entity.id as string, entity])
	);

	return nextIds
		.map((id) => pageById.get(id) ?? previousById.get(id))
		.filter((entity): entity is TEntity => entity !== undefined);
}

interface SingleLookupChangeCallbacks<TEntity extends IEntity> {
	onChange?: (entity?: TEntity) => void;
	onChangeId?: (id?: string) => void;
	onFieldChange?: (id: string | null) => void;
}

/**
 * Уведомляет об изменении single lookup ровно одним способом.
 * Хук useLookup связывает onChangeId с onChange, поэтому нельзя вызывать их оба.
 */
export function notifySingleLookupChange<TEntity extends IEntity>(
	id: string | null,
	entity: TEntity | null,
	{ onChange, onChangeId, onFieldChange }: SingleLookupChangeCallbacks<TEntity>
): void {
	if (onChange) {
		onChange(entity ?? undefined);
	} else if (onChangeId) {
		onChangeId(id ?? undefined);
	} else {
		onFieldChange?.(id);
	}
}

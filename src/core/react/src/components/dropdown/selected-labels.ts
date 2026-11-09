import type { DropdownItem } from './types';

/**
 * Кэш подписей только для текущих выбранных ID.
 * Фильтрация или загрузка следующей серверной выдачи не должна стирать подпись.
 */
export function rememberSelectedLabels(
	previous: ReadonlyMap<string, string>,
	selectedIds: readonly string[],
	items: readonly Pick<DropdownItem, 'value' | 'label'>[]
): Map<string, string> {
	const current = new Map(items.map(({ value, label }) => [value, label]));
	const result = new Map<string, string>();
	for (const id of selectedIds) {
		const label = current.get(id) ?? previous.get(id);
		if (label !== undefined) {
			result.set(id, label);
		}
	}
	return result;
}

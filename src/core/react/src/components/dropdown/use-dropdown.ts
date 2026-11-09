import { useMemo } from 'react';
import type { IEntity } from '../../models/entity';
import type { DropdownItem } from './types';

export function useDropdown<T extends IEntity>(
	options: Record<string, string> | T[],
	getOptionLabel?: (option: T) => string
) {
	const isEnum = !Array.isArray(options);

	const items = useMemo<DropdownItem<T>[]>(() => {
		if (isEnum) {
			return Object.values(options as Record<string, string>).map((label) => ({
				value: label,
				label,
				object: null,
			}));
		}

		return (options as T[]).map((opt) => ({
			value: opt.id ?? '',
			label: getOptionLabel!(opt),
			object: opt,
		}));
	}, [getOptionLabel, isEnum, options]);

	const itemsMap = useMemo<Record<string, DropdownItem<T>>>(() => {
		const map: Record<string, DropdownItem<T>> = {};
		items.forEach((item) => {
			map[item.value] = item;
		});
		return map;
	}, [items]);

	return { isEnum, items, itemsMap };
}

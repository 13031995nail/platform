import { useCallback, useMemo } from "react";
import { EditIcon, TrashIcon } from "../ui/icon";
import type { IEntity } from "../../models/entity";
import type { EntityListViewButton } from "./entity-list-types";

export interface UseEntityRowActionsProps<TEntity extends IEntity> {
	buttons?: EntityListViewButton<TEntity>[];
	onRowClick?: (rowData: TEntity) => void;
	showEditMenuItemInRow?: boolean;
	showDeleteMenuItemInRow?: boolean;
	deleteEntity: (id?: string) => void | Promise<void>;
}

export interface EntityRowActions<TEntity extends IEntity> {
	rowButtons: EntityListViewButton<TEntity>[];
	hasRowActions: boolean;
}

/**
 * Действия строки списка: пользовательские и системные пункты меню.
 */
export function useEntityRowActions<TEntity extends IEntity>({
	buttons = [],
	onRowClick,
	showEditMenuItemInRow = true,
	showDeleteMenuItemInRow = true,
	deleteEntity,
}: UseEntityRowActionsProps<TEntity>): EntityRowActions<TEntity> {
	const handleDelete = useCallback(async (rowData: TEntity) => {
		try {
			await deleteEntity(rowData.id);
		} catch (error) {
			console.error("Ошибка при удалении:", error);
		}
	}, [deleteEntity]);

	const rowButtons = useMemo(() => {
		const result = [...buttons];
		result.push({
			name: "Редактировать",
			action: onRowClick,
			isVisible: showEditMenuItemInRow,
			icon: EditIcon,
		});
		result.push({
			name: "Удалить",
			action: handleDelete,
			isVisible: showDeleteMenuItemInRow,
			icon: TrashIcon,
			isDeleteAction: true,
		});
		return result;
	}, [
		buttons,
		handleDelete,
		onRowClick,
		showDeleteMenuItemInRow,
		showEditMenuItemInRow,
	]);

	return {
		rowButtons,
		hasRowActions: rowButtons.some(button => button.isVisible),
	};
}

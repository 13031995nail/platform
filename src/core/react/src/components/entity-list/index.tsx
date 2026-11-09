import { TmobisButton } from '@tmobis/core/components/button';
import { TmobisSpinner } from '@tmobis/core/components/spinner';
import { Children, useMemo } from 'react';
import { StyleSheet, View } from 'react-native';
import type { IEntity } from "../../models/entity";
import { HStack } from "../ui/hstack";
import { EntityListController } from "./entity-list-controller";
import type { EntityListProps } from "./entity-list-types";
import EntityListView from "./entity-list-view";
import EntityListToolbar from "./toolbar";

export type { EntityId } from "./use-entity-list";
export type {
	EntityListProps,
	EntityListRowConfig,
	EntityListViewButton,
	EntityListViewColumn,
} from "./entity-list-types";
export type { EntityListSystemMenuItem } from "./toolbar-shared";
export { EntityListController } from "./entity-list-controller";
export type { EntityListControllerProps } from "./entity-list-controller";
export { useEntitySelection } from "./use-entity-selection";
export type {
	EntitySelection,
	UseEntitySelectionProps,
} from "./use-entity-selection";
export { useEntityRowActions } from "./use-entity-row-actions";
export type {
	EntityRowActions,
	UseEntityRowActionsProps,
} from "./use-entity-row-actions";
export { default as EntityListView } from "./entity-list-view";
export { default as EntityListToolbar } from "./toolbar";

/**
 * Компонент списка сущностей (facade).
 * Для отображения требуются данные из хука {@link useEntityList}.
 * Внутри использует {@link EntityListController}, {@link EntityListToolbar} и {@link EntityListView}.
 *
 * Поддерживается:
 * - Сортировка колонок (по клику на заголовок)
 * - Постраничная навигация
 * - Множественный выбор записей и операции над ними
 * - Фильтрация и поиск
 * @constructor
 */
export default function EntityList<TEntity extends IEntity>({
	children,
	hasFilter = false,
	showToolbar = true,
	...controllerProps
}: EntityListProps<TEntity>) {
	const {
		listData: {
			isLoading,
			filter: { applyFilter },
		},
	} = controllerProps;

	const hasTextFilters = Children.count(children) > 0;

	const headerSlot = useMemo(() => {
		if (!hasTextFilters && !showToolbar) return null;
		return (
			<View>
				{hasTextFilters && (
					hasFilter ? (
						<HStack className="mb-4 flex-wrap items-end gap-3">
							{children}
							<TmobisButton
								action="primary"
								label="Применить"
								size="sm"
								className="shrink-0 self-end"
								onPress={applyFilter}
							/>
						</HStack>
					) : (
						children
					)
				)}
				{showToolbar && <EntityListToolbar />}
			</View>
		);
	}, [children, hasFilter, showToolbar, applyFilter, hasTextFilters]);

	if (isLoading) {
		return <TmobisSpinner />;
	}

	return (
		<EntityListController {...controllerProps} hasFilter={hasFilter}>
			<View style={styles.container}>
				<EntityListView headerSlot={headerSlot} />
			</View>
		</EntityListController>
	);
}

const styles = StyleSheet.create({
	container: {
		flex: 1,
		paddingTop: 16,
		paddingHorizontal: 16,
	},
});
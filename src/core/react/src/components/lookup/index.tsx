import {
	TmobisFormControlLabel,
	TmobisFormControlMessage
} from '@tmobis/core/components/form-control';
import { TmobisInput } from "@tmobis/core/components/input";
import { TmobisModal } from "@tmobis/core/components/modal";
import { IInputFieldProps } from '@tmobis/core/components/ui/input';
import { IEntity } from "@tmobis/core/models/entity";
import { useCallback, useMemo, useRef, useState } from "react";
import { FieldValues } from 'react-hook-form';
import { TmobisSimpleDropdown } from '../dropdown';
import type {
	DropdownArrayMultiProps,
	DropdownArraySingleProps,
} from '../dropdown/types';
import { EditFormFieldProps } from "../entity-edit";
import { useEntityEditField, useEntityEditFieldState } from "../entity-edit/context";
import EntityList, { EntityListViewColumn } from "../entity-list";
import { EntityListData } from "../entity-list/use-entity-list";
import { FormControl } from "../ui/form-control";
import { LookupData } from "./hook";
import { mergeSelectedEntities, notifySingleLookupChange } from "./selection";

type LookupDisplayFieldProps = {
	columns: EntityListViewColumn[];
	displayField: string;
	placeholder: string;
};

type LookupSharedProps =
	& Omit<IInputFieldProps, 'value'>
	& EditFormFieldProps
	& LookupDisplayFieldProps
	& {
		listData: EntityListData<any>;
		onRemove?: () => void;
	};

type TmobisLookupSingleProps<TEntity extends IEntity> =
	& LookupSharedProps
	& Omit<LookupData<TEntity>, 'fieldName' | 'listData' | 'onRemove' | 'onChange' | 'onChangeId'>
	& {
		multiple?: false;
		value?: TEntity | undefined;
		onChange?: (value?: TEntity) => void;
		onChangeId?: (value?: string) => void;
	};

type TmobisLookupMultiProps<TEntity extends IEntity> =
	& LookupSharedProps
	& {
		multiple: true;
		value?: TEntity[];
		valueId?: string[];
		onChange?: (value: TEntity[]) => void;
		onChangeId?: (value: string[]) => void;
	};

type TmobisLookupProps<TEntity extends IEntity> =
	| TmobisLookupSingleProps<TEntity>
	| TmobisLookupMultiProps<TEntity>;

type TmobisLookupDropdownSingleProps<TEntity extends IEntity> =
	& Omit<TmobisLookupSingleProps<TEntity>, 'columns' | 'valueId'>
	& {
		valueId?: string;
		label?: string;
		isDisabled?: boolean;
		helperText?: string;
	};

type TmobisLookupDropdownMultiProps<TEntity extends IEntity> =
	& Omit<TmobisLookupMultiProps<TEntity>, 'columns'>
	& {
		label?: string;
		isDisabled?: boolean;
		helperText?: string;
	};

type TmobisLookupDropdownProps<TEntity extends IEntity> =
	| TmobisLookupDropdownSingleProps<TEntity>
	| TmobisLookupDropdownMultiProps<TEntity>;

function getEntityDisplayLabel<TEntity extends IEntity>(
	entity: TEntity,
	displayField: string
): string {
	const label = entity[displayField as keyof TEntity];
	return label == null ? '' : String(label);
}

function formatEntitiesLabel<TEntity extends IEntity>(
	entities: TEntity[] | undefined,
	displayField: string
): string {
	if (!entities?.length) {
		return '';
	}
	return entities
		.map((entity) => getEntityDisplayLabel(entity, displayField))
		.filter(Boolean)
		.join(', ');
}

function resolveSelectedIds<TEntity extends IEntity>(
	value?: TEntity[],
	valueId?: string[]
): string[] {
	if (value !== undefined) {
		return value
			.map((entity) => entity.id)
			.filter((id): id is string => Boolean(id));
	}
	return valueId ?? [];
}

function TmobisLookup<TEntity extends IEntity & FieldValues>(
	props: TmobisLookupProps<TEntity>
) {
	const isMultiple = props.multiple === true;
	const {
		isRequired,
		displayField,
		columns,
		listData,
		fieldName,
		validationRules,
		label,
		placeholder,
		onRemove,
	} = props;
	const [isModalVisible, setIsModalVisible] = useState(false);

	const field = useEntityEditField({
		fieldName: isMultiple ? undefined : fieldName,
		isRequired: isMultiple ? undefined : isRequired,
		validationRules: isMultiple ? undefined : validationRules,
	});

	const selectedEntities = useMemo(() => {
		if (isMultiple) {
			return (props as TmobisLookupMultiProps<TEntity>).value ?? [];
		}
		const singleProps = props as TmobisLookupSingleProps<TEntity>;
		const selectedValue = singleProps.value !== undefined
			? singleProps.value
			: field?.value;
		return selectedValue ? [selectedValue as TEntity] : [];
	}, [field?.value, isMultiple, props]);

	const selectedIds = useMemo(() => {
		if (!isMultiple) {
			return selectedEntities
				.map((entity) => entity.id)
				.filter((id): id is string => Boolean(id));
		}
		const multiProps = props as TmobisLookupMultiProps<TEntity>;
		return resolveSelectedIds(multiProps.value, multiProps.valueId);
	}, [isMultiple, props, selectedEntities]);

	const resolvedValue = isMultiple
		? formatEntitiesLabel(selectedEntities, displayField)
		: (selectedEntities[0]
			? getEntityDisplayLabel(selectedEntities[0], displayField)
			: '');

	const handleRowClick = useCallback((entity: TEntity) => {
		if (isMultiple) {
			return;
		}
		(props as TmobisLookupSingleProps<TEntity>).onChange?.(entity);
		setIsModalVisible(false);
	}, [isMultiple, props]);

	const handleSelectedIdsChange = useCallback((
		nextIds: string[],
		pageEntities: TEntity[]
	) => {
		if (!isMultiple) {
			return;
		}
		const multiProps = props as TmobisLookupMultiProps<TEntity>;
		const previous = multiProps.value ?? [];
		const nextEntities = mergeSelectedEntities(previous, nextIds, pageEntities);
		multiProps.onChange?.(nextEntities);
		multiProps.onChangeId?.(nextIds);
	}, [isMultiple, props]);

	const handleRemove = useCallback(() => {
		if (isMultiple) {
			const multiProps = props as TmobisLookupMultiProps<TEntity>;
			multiProps.onChange?.([]);
			multiProps.onChangeId?.([]);
			onRemove?.();
			return;
		}
		onRemove?.();
	}, [isMultiple, onRemove, props]);

	return (
		<>
			<TmobisInput
				fieldName={isMultiple ? undefined : fieldName}
				onRemove={handleRemove}
				value={resolvedValue}
				placeholder={placeholder}
				isReadonly={true}
				rightIconName="MenuIcon"
				onRightIconClick={() => setIsModalVisible(true)}
				label={label}
				isRequired={isRequired}
			/>
			<TmobisModal
				title={label}
				size='xlg'
				isOpen={isModalVisible}
				isDenyVisible={false}
				isApproveVisible={false}
				onClose={() => setIsModalVisible(false)}
			>
				<>
					<EntityList
						onRowClick={isMultiple ? undefined : handleRowClick}
						columns={columns}
						listData={listData}
						getRowConfig={(entity) => ({
							mobileCardTitle: entity[displayField],
							mobileCardTitleColumn: displayField,
						})}
						hasSelection={isMultiple}
						selectedIds={isMultiple ? selectedIds : undefined}
						onSelectedIdsChange={
							isMultiple ? handleSelectedIdsChange : undefined
						}
						showDeleteMenuItemInRow={false}
						showEditMenuItemInRow={false}
						showToolbar={false}
					/>
				</>
			</TmobisModal>
		</>
	);
}

const LOOKUP_SEARCH_DEBOUNCE_MS = 300;
const LOOKUP_MIN_SEARCH_LENGTH = 2;

function TmobisDropdownLookup<TEntity extends IEntity & FieldValues>(
	props: TmobisLookupDropdownProps<TEntity>
) {
	const isMultiple = props.multiple === true;
	const {
		isRequired,
		value,
		label,
		placeholder,
		displayField,
		listData,
		fieldName,
		validationRules,
		isDisabled,
		helperText,
	} = props;
	const { data, isLoading, filter } = listData;
	const { applyFilterValues, activeFilter, activeSearch } = filter;
	const activeSearchRef = useRef(activeSearch);
	activeSearchRef.current = activeSearch;

	const {
		field,
		errorText,
		isInvalid
	} = useEntityEditFieldState({
		fieldName: isMultiple ? undefined : fieldName,
		isRequired: isMultiple ? undefined : isRequired,
		validationRules: isMultiple ? undefined : validationRules,
	});

	const multiProps = props as TmobisLookupDropdownMultiProps<TEntity>;
	const singleProps = props as TmobisLookupDropdownSingleProps<TEntity>;
	const selectedEntities = isMultiple ? (multiProps.value ?? []) : [];
	const selectedIds = isMultiple
		? resolveSelectedIds(multiProps.value, multiProps.valueId)
		: [];

	const handleMultiChange = useCallback((
		ids: string[] | null,
		pageEntities: TEntity[]
	) => {
		const nextIds = ids ?? [];
		// Результат поиска содержит только текущую выдачу.
		// Сохраняем сущности, выбранные в предыдущих выдачах.
		const nextEntities = mergeSelectedEntities(selectedEntities, nextIds, pageEntities);
		multiProps.onChange?.(nextEntities);
		multiProps.onChangeId?.(nextIds);
	}, [multiProps, selectedEntities]);

	const handleSingleChange = useCallback((
		id: string | null,
		entity: TEntity | null
	) => {
		notifySingleLookupChange(id, entity, {
			onChange: singleProps.onChange,
			onChangeId: singleProps.onChangeId,
			onFieldChange: field?.onChange,
		});
	}, [field, singleProps]);

	const handleSearch = useCallback((query: string | null) => {
		if (!query && !activeSearchRef.current) {
			return;
		}
		applyFilterValues(activeFilter ?? {}, query ?? undefined);
	}, [activeFilter, applyFilterValues]);

	const formatSelectedLabel = useCallback(
		(items: Array<{ value: string; label: string }>) => {
			if (isMultiple) {
				if (selectedEntities.length > 0) {
					return formatEntitiesLabel(selectedEntities, displayField);
				}
				return items.map((item) => item.label).filter(Boolean).join(', ');
			}
			if (value) {
				return getEntityDisplayLabel(value as TEntity, displayField);
			}
			return items.map((item) => item.label).filter(Boolean).join(', ');
		},
		[displayField, isMultiple, selectedEntities, value]
	);

	const getOptionLabel = useCallback(
		(option: TEntity) => getEntityDisplayLabel(option, displayField),
		[displayField]
	);

	const searchProps = {
		searchable: true as const,
		searchPlacement: 'panel' as const,
		minSearchLength: LOOKUP_MIN_SEARCH_LENGTH,
		searchDebounceMs: LOOKUP_SEARCH_DEBOUNCE_MS,
		searchPlaceholder: 'Поиск...',
		onSearch: handleSearch,
		loading: isLoading,
	};

	if (isMultiple) {
		const dropdownProps: DropdownArrayMultiProps<TEntity> = {
			multiple: true,
			options: data ?? [],
			value: selectedIds,
			placeholder,
			isDisabled,
			getOptionLabel,
			formatSelectedLabel,
			onChange: handleMultiChange,
			...searchProps,
		};

		return (
			<FormControl
				isDisabled={isDisabled}
				isRequired={isRequired}
			>
				<TmobisFormControlLabel label={label} />
				<TmobisSimpleDropdown {...dropdownProps} />
				{helperText ? (
					<TmobisFormControlMessage
						helperText={helperText}
						isInvalid={false}
					/>
				) : null}
			</FormControl>
		);
	}

	const selectedValueId = singleProps.valueId !== undefined
		? singleProps.valueId
		: field?.value;

	const singleDropdownProps: DropdownArraySingleProps<TEntity> = {
		options: data ?? [],
		value: selectedValueId ?? null,
		placeholder,
		isDisabled,
		isInvalid,
		getOptionLabel,
		formatSelectedLabel,
		defaultValue: value
			? getEntityDisplayLabel(value as TEntity, displayField)
			: undefined,
		onChange: handleSingleChange,
		...searchProps,
	};

	return (
		<FormControl
			isInvalid={isInvalid}
			isDisabled={isDisabled}
			isRequired={isRequired}
		>
			<TmobisFormControlLabel label={label} />
			<TmobisSimpleDropdown {...singleDropdownProps} />
			<TmobisFormControlMessage
				helperText={helperText}
				isInvalid={isInvalid}
				errorText={errorText}
			/>
		</FormControl>
	);
}

export {
	TmobisDropdownLookup,
	TmobisLookup,
	type TmobisLookupDropdownProps,
	type TmobisLookupProps,
};

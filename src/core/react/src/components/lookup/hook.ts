import { IEntity } from "@tmobis/core/models/entity";
import { useCallback } from "react";
import { FieldValues, Path, UseFormReturn } from "react-hook-form";
import { UseEntityEditData } from "../entity-edit/use-entity-edit";
import useEntityList, { EntityListData, UseEntityListProps } from "../entity-list/use-entity-list";

interface Props<TMaster extends FieldValues>
	extends Omit<UseEntityListProps, 'useUrlParams'> {
	form?: UseEntityEditData<TMaster>;
	associationName?: Path<TMaster>;
}

interface LookupData<TEntity extends IEntity> {
	fieldName?: string;
	listData: EntityListData<TEntity>;
	value?: TEntity;
	valueId?: string;
	onChange: (value?: TEntity) => void;
	onChangeId: (value?: string) => void;
	onRemove: () => void;
}

function getFormAssociation<
	TEntity extends IEntity & FieldValues,
	TMaster extends FieldValues
>(
	associationName?: Path<TMaster>,
	form?: UseFormReturn<TMaster>
) {
	return (associationName && form)
		? form?.getValues()[associationName] as TEntity
		: undefined
}

function useLookup<TEntity extends IEntity & FieldValues,
	TMaster extends FieldValues
>({
	form: editForm, associationName, ...props
}: Props<TMaster>): LookupData<TEntity> {
	const listData = useEntityList<TEntity>({ ...props, useUrlParams: false });
	const form = editForm?.form;
	const associationId = associationName
		? `${associationName}Id` as Path<TMaster>
		: undefined;

	const value = getFormAssociation<TEntity, TMaster>(associationName, form);

	const onChange = useCallback((newValue?: TEntity) => {
		if (associationName && form) {
			form.setValue(associationName, newValue as any, {
				shouldDirty: true,
				shouldValidate: true
			});
		}

		if (associationId && form) {
			form.setValue(associationId, newValue?.id as any, {
				shouldDirty: true,
				shouldValidate: true
			});
		}
	}, [associationId, associationName, form]);

	const onChangeId = useCallback((newValueId?: string) => {
		const newValue = listData?.data?.find(entity => entity.id === newValueId);
		onChange(newValue);
	}, [listData?.data, onChange]);

	const onRemove = useCallback(() => {
		onChange(null);
	}, [onChange]);

	return {
		fieldName: associationName,
		listData,
		value,
		valueId: value?.id,
		onChange,
		onChangeId,
		onRemove
	}
}

function useDropdownLookup<TEntity extends IEntity & FieldValues,
	TMaster extends FieldValues
>(props: Props<TMaster>) {
	const lookupData = useLookup<TEntity, TMaster>({
		...props,
		defaultPerPage: 100,
		withCount: false,
	});
	const associationId = props.associationName
		? `${props.associationName}Id`
		: undefined;

	return {
		...lookupData,
		fieldName: associationId
	};
}

export {
	useDropdownLookup,
	useLookup,
	type LookupData
};


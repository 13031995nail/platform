'use client';

import {
	TmobisFormControlLabel,
	TmobisFormControlMessage,
} from '@tmobis/core/components/form-control';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import { type ReactNode } from 'react';
import type { IEntity } from '../../models/entity';
import { useEntityEditFieldState } from '../entity-edit/context';
import { TmobisSimpleDropdown } from './simple-dropdown';
import type {
	DropdownArrayFormProps,
	DropdownArrayMultiFormProps,
	DropdownArrayMultiProps,
	DropdownArraySingleProps,
	DropdownEnumMultiFormProps,
	DropdownEnumMultiProps,
	DropdownEnumSingleProps,
	TmobisDropdownProps,
} from './types';

export type {
	DropdownItem,
	DropdownProps,
	TmobisDropdownProps,
	TmobisSimpleDropdownProps,
} from './types';
export { useDropdown } from './use-dropdown';
export { TmobisSimpleDropdown } from './simple-dropdown';

function TmobisDropdown<
	T extends IEntity,
	E extends Record<string, string> = Record<string, string>,
>(props: TmobisDropdownProps<T, E>) {
	const {
		fieldName,
		validationRules,
		isRequired,
		label,
		placeholder,
		options,
		multiple,
		searchable,
		onSearch,
		onOpenChange,
		value: valueProp,
		onChange: onChangeProp,
		helperText,
		isDisabled,
		...rest
	} = props;

	const { field, errorText, isInvalid } = useEntityEditFieldState({
		fieldName,
		isRequired,
		validationRules,
	});

	const isEnum = !Array.isArray(options);

	const simpleProps = {
		...rest,
		placeholder,
		searchable,
		onSearch,
		onOpenChange,
		isDisabled,
		isInvalid,
	};

	let control: ReactNode;

	if (multiple && isEnum) {
		const enumProps = props as DropdownEnumMultiFormProps<E>;
		control = (
			<TmobisSimpleDropdown
				{...simpleProps}
				multiple
				options={enumProps.options}
				value={valueProp ?? field?.value ?? null}
				onChange={((value) => {
					(onChangeProp as DropdownEnumMultiProps<E>['onChange'] | undefined)?.(
						value
					);
					field?.onChange(value);
				}) as DropdownEnumMultiProps<E>['onChange']}
			/>
		);
	} else if (multiple) {
		const arrayProps = props as DropdownArrayMultiFormProps<T>;
		control = (
			<TmobisSimpleDropdown
				{...simpleProps}
				multiple
				options={arrayProps.options}
				getOptionLabel={arrayProps.getOptionLabel}
				value={valueProp ?? field?.value ?? null}
				onChange={((ids, selectedOptions) => {
					(
						onChangeProp as DropdownArrayMultiProps<T>['onChange'] | undefined
					)?.(ids, selectedOptions);
					field?.onChange(ids);
				}) as DropdownArrayMultiProps<T>['onChange']}
			/>
		);
	} else if (isEnum) {
		control = (
			<TmobisSimpleDropdown
				{...simpleProps}
				options={options as E}
				value={valueProp ?? field?.value ?? null}
				onChange={((value) => {
					(onChangeProp as DropdownEnumSingleProps<E>['onChange'] | undefined)?.(
						value
					);
					field?.onChange(value);
				}) as DropdownEnumSingleProps<E>['onChange']}
			/>
		);
	} else {
		const arrayProps = props as DropdownArrayFormProps<T>;
		control = (
			<TmobisSimpleDropdown
				{...simpleProps}
				options={arrayProps.options}
				getOptionLabel={arrayProps.getOptionLabel}
				value={valueProp ?? field?.value ?? null}
				onChange={((id, option) => {
					(
						onChangeProp as DropdownArraySingleProps<T>['onChange'] | undefined
					)?.(id, option);
					field?.onChange(id);
				}) as DropdownArraySingleProps<T>['onChange']}
			/>
		);
	}

	return (
		<FormControl
			isInvalid={isInvalid}
			isDisabled={isDisabled}
			isRequired={isRequired}
		>
			<TmobisFormControlLabel label={label} />
			{control}
			<TmobisFormControlMessage
				helperText={helperText}
				isInvalid={isInvalid}
				errorText={errorText}
			/>
		</FormControl>
	);
}

export { TmobisDropdown };
export default TmobisDropdown;

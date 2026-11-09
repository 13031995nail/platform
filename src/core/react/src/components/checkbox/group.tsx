import {
	TmobisSimpleCheckbox,
	TmobisSimpleRadio,
	type TmobisSimpleCheckboxProps,
	type TmobisSimpleRadioProps
} from '@tmobis/core/components/checkbox';
import { EditFormFieldProps } from '@tmobis/core/components/entity-edit';
import { useEntityEditFieldState } from '@tmobis/core/components/entity-edit/context';
import {
	TmobisFormControlLabel,
	TmobisFormControlMessage
} from '@tmobis/core/components/form-control';
import { CheckboxGroup } from '@tmobis/core/components/ui/checkbox';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import { RadioGroup, type IRadioGroupProps } from '@tmobis/core/components/ui/radio';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { useState } from 'react';

type TmobisCheckboxGroupProps =
	IRadioGroupProps &
	EditFormFieldProps & {
		variant?: 'checkbox' | 'radio';
		values: string[];
		helperText?: string;
		items: TmobisSimpleCheckboxProps[] | TmobisSimpleRadioProps[];
		onChange?: (keys: string[]) => void;
	}

const TmobisCheckboxGroup = ({
	variant = 'checkbox',
	label,
	values: initialValues,
	helperText,
	items,
	onChange,
	fieldName,
	isRequired,
	validationRules,
	...props
}: TmobisCheckboxGroupProps) => {
	const isCheckbox = variant === 'checkbox';

	const {
		field,
		errorText,
		isInvalid
	} = useEntityEditFieldState({
		fieldName,
		isRequired,
		defaultValue: initialValues,
		validationRules,
	});

	const fieldValue = Array.isArray(field?.value) ? field.value : [];
	const [localValues, setLocalValues] = useState(initialValues);
	const values = field ? fieldValue : localValues;

	const handleChange = (incomingValue: string | string[]) => {
		const updatedValues = Array.isArray(incomingValue) ? incomingValue : [incomingValue];

		if (field) {
			field.onChange(updatedValues);
		} else {
			setLocalValues(updatedValues);
		}
		onChange?.(updatedValues);
	};

	const CheckboxGroupComponent = (
		<CheckboxGroup
			value={values}
			onChange={(keys: string[]) => handleChange(keys)}
			{...props}
		>
			<VStack space="md">
				{items.map((item: TmobisSimpleCheckboxProps | TmobisSimpleRadioProps) =>
					<TmobisSimpleCheckbox groupVariant={variant} {...item} />
				)}
			</VStack>
		</CheckboxGroup>
	)
	const RadioGroupComponent = (
		<RadioGroup
			value={values[0]}
			onChange={(key: string) => handleChange(key)}
			{...props}
		>
			<VStack space="md">
				{items.map((item: TmobisSimpleCheckboxProps | TmobisSimpleRadioProps) =>
					<TmobisSimpleRadio groupVariant={variant} {...item} />
				)}
			</VStack>
		</RadioGroup>
	)

	return (
		<FormControl>
			<VStack space="md">
				<TmobisFormControlLabel label={label} />
				{isCheckbox
					? CheckboxGroupComponent
					: RadioGroupComponent
				}
				<TmobisFormControlMessage
					helperText={helperText}
					isInvalid={isInvalid}
					errorText={errorText}
				/>
			</VStack>
		</FormControl>
	);
};

export { TmobisCheckboxGroup };

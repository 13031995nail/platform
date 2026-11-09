import { EditFormFieldProps } from '@tmobis/core/components/entity-edit';
import { useEntityEditFieldState } from '@tmobis/core/components/entity-edit/context';
import { TmobisFormControlMessage } from '@tmobis/core/components/form-control';
import {
	Checkbox,
	CheckboxIcon,
	CheckboxIndicator,
	CheckboxLabel,
	ICheckboxProps,
	checkboxLabelStyle
} from '@tmobis/core/components/ui/checkbox';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { CheckIcon, CircleIcon } from '@tmobis/core/components/ui/icon';
import {
	Radio,
	RadioIcon,
	RadioIndicator,
	RadioLabel,
	type IRadioProps
} from '@tmobis/core/components/ui/radio';
import { Switch, type ISwitchProps } from '@tmobis/core/components/ui/switch';
import { Text } from '@tmobis/core/components/ui/text';
import {ReactNode, useState} from 'react';

type TmobisSimpleCheckboxProps = ICheckboxProps & {
	label?: string;
}
type TmobisSimpleRadioProps = IRadioProps & {
	label?: string;
}
type TmobisSimpleSwitchProps = ISwitchProps & {
	label?: string;
}

type TmobisSwitchProps =
	TmobisSimpleSwitchProps &
	TmobisSimpleCheckboxProps &
	EditFormFieldProps & {
		variant?: 'switch' | 'checkbox'
	}

const TmobisSimpleCheckbox = ({
	label,
	isChecked,
	isDisabled = false,
	onChange,
	...props
}: TmobisSimpleCheckboxProps) => {
	return (
		<Checkbox
			isChecked={isChecked}
			isDisabled={isDisabled}
			onChange={onChange}
			{...props}
		>
			<CheckboxIndicator>
				<CheckboxIcon as={CheckIcon} />
			</CheckboxIndicator>
			{label
				? <CheckboxLabel>{label}</CheckboxLabel>
				: null
			}
		</Checkbox>
	)
};

const TmobisSimpleRadio = ({
	label,
	isChecked,
	isDisabled = false,
	...props
}: TmobisSimpleRadioProps) => {
	return (
		<Radio isChecked={isChecked} isDisabled={isDisabled} {...props}>
			<RadioIndicator>
				<RadioIcon as={CircleIcon} />
			</RadioIndicator>
			{label
				? <RadioLabel>{label}</RadioLabel>
				: null
			}
		</Radio>
	)
};

const TmobisSimpleSwitch = ({
	label,
	onChange,
	...props
}: TmobisSimpleSwitchProps) => {
	const switchLabelStyle = checkboxLabelStyle({});

	return (
		<HStack space="sm">
			<Switch
				onToggle={onChange}
				{...props}
			/>
			{label
				? <Text className={switchLabelStyle}>{label}</Text>
				: null
			}
		</HStack>
	)
};

const TmobisSwitch = ({
	variant = 'switch',

	isChecked,
	value,
	helperText,
	isDisabled = false,
	isRequired = false,

	fieldName,
	defaultValue = false,
	validationRules,
	onChange,

	...props
}: TmobisSwitchProps) => {
	const {
		field,
		errorText,
		isInvalid
	} = useEntityEditFieldState({
		fieldName,
		isRequired,
		defaultValue,
		validationRules,
	});

	const isSwitch = variant === 'switch';

	const [localIsChecked, setLocalIsChecked] = useState(defaultValue);
	const fieldValue = typeof field?.value === 'boolean' ? field.value : null;

	const resolvedIsChecked = isChecked ?? fieldValue ?? localIsChecked;
	const resolvedValue = value ?? field?.value;

	const handleChange = (value: boolean) => {
		if (field) {
			field.onChange(value);
		} else if (isChecked === undefined || isChecked === null) {
			setLocalIsChecked(value);
		}
		onChange?.(value);
	};

	const TmobisSimpleSwitchComponent = (
		<TmobisSimpleSwitch
			ref={field?.ref}
			value={resolvedValue}
			isChecked={resolvedIsChecked}
			isDisabled={isDisabled}
			onChange={handleChange}
			{...props}
		/>
	);
	const TmobisSimpleCheckboxComponent = (
		<TmobisSimpleCheckbox
			ref={field?.ref}
			value={resolvedValue}
			isChecked={resolvedIsChecked}
			isDisabled={isDisabled}
			onChange={handleChange}
			{...props}
		/>
	);

	return (
		<FormControl
			isInvalid={isInvalid}
			isDisabled={isDisabled}
			isRequired={isRequired}
		>
			{isSwitch
				? TmobisSimpleSwitchComponent
				: TmobisSimpleCheckboxComponent
			}
			<TmobisFormControlMessage
				helperText={helperText}
				isInvalid={isInvalid}
				errorText={errorText}
			/>
		</FormControl>
	);
};

export {
	TmobisSimpleCheckbox,
	TmobisSimpleRadio,
	TmobisSwitch,
	type TmobisSimpleCheckboxProps,
	type TmobisSimpleRadioProps
};

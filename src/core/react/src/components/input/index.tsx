import type { EditFormFieldProps } from '@tmobis/core/components/entity-edit';
import { useEntityEditFieldState } from '@tmobis/core/components/entity-edit/context';
import {
	TmobisFormControlLabel,
	TmobisFormControlMessage
} from '@tmobis/core/components/form-control';
import { TmobisIcon } from '@tmobis/core/components/icon';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import {
	IInputFieldProps,
	Input,
	InputField,
	inputIconStyle,
	InputSlot
} from '@tmobis/core/components/ui/input';
import { Spinner } from '@tmobis/core/components/ui/spinner';
import { useCallback } from 'react';

type TmobisSimpleInputProps = IInputFieldProps & {
	variant?: 'outline' | 'underlined';

	placeholder?: string;
	leftIconName?: string;
	rightIconName?: string;

	isInvalid?: boolean;
	isDisabled?: boolean;
	isReadonly?: boolean;
	isLoading?: boolean;
	hiddenClearIcon?: boolean;
	showClearIcon?: boolean;

	onLeftIconClick?: () => void;
	onRightIconClick?: () => void;
	onRemove?: () => void;
};
type TmobisInputProps =
	Omit<TmobisSimpleInputProps, 'isInvalid'> &
	EditFormFieldProps & {
		helperText?: string;
		containerClassName?: string;
	};

const TmobisSimpleInput = ({
	variant = 'outline',
	size = 'md',

	placeholder = 'Введите значение',
	leftIconName,
	rightIconName,

	isInvalid = false,
	isDisabled = false,
	isReadonly = false,
	isLoading = false,
	hiddenClearIcon = false,
	showClearIcon,

	value,
	onBlur,
	onChangeText,
	onRemove,
	onLeftIconClick,
	onRightIconClick,

	...props
}: TmobisSimpleInputProps) => {

	const handleRemove = useCallback(() => {
		if (onRemove) {
			onRemove?.();
		} else if (onChangeText) {
			onChangeText?.('');
		}
	}, [onRemove, onChangeText]);

	const shouldShowClear =
		showClearIcon !== undefined
			? showClearIcon
			: Boolean(value) && !hiddenClearIcon;

	return (
		<Input
			isDisabled={isDisabled}
			isInvalid={isInvalid}
		>
			{leftIconName
				? <InputSlot
					onPress={onLeftIconClick}
					accessible={!onLeftIconClick}
					accessibilityLabel={leftIconName}
				>
					<TmobisIcon name={leftIconName} size="lg" className={inputIconStyle({ size: 'lg' })} />
				</InputSlot>
				: null
			}
			<InputField
				placeholder={placeholder}
				editable={!isDisabled && !isReadonly}
				value={value}
				onBlur={onBlur}
				onChangeText={onChangeText}
				{...props}
			/>
			{isLoading
				? <InputSlot>
					<Spinner size="small" />
				</InputSlot>
				: null
			}
			{shouldShowClear
				? <InputSlot
					onPress={handleRemove}
					accessible={!handleRemove}
					accessibilityLabel="Clear"
				>
					<TmobisIcon name={"CloseIcon"} size="lg" className={inputIconStyle({ size: 'lg' })} />
				</InputSlot>
				: null
			}
			{rightIconName
				? <InputSlot
					onPress={onRightIconClick}
					accessible={!onRightIconClick}
					accessibilityLabel={rightIconName}
				>
					<TmobisIcon name={rightIconName} size="lg" className={inputIconStyle({ size: 'lg' })} />
				</InputSlot>
				: null
			}
		</Input>
	);
};
const TmobisInput = ({
	size = 'md',
	containerClassName,

	label,
	helperText,
	isDisabled = false,
	isRequired = false,
	isReadonly = false,

	fieldName,
	validationRules,
	value,
	onBlur,
	onChangeText,
	onRemove,

	...props
}: TmobisInputProps) => {
	const {
		field,
		errorText,
		isInvalid
	} = useEntityEditFieldState({
		fieldName,
		isRequired,
		validationRules,
	});

	const handleBlur = useCallback(() => {
		onBlur?.();
		field?.onBlur();
	}, [onBlur]);

	const handleChangeText = useCallback((event: any) => {
		onChangeText?.(event);
		field?.onChange(event);
	}, [onChangeText]);

	return (
		<FormControl
			isInvalid={isInvalid}
			isDisabled={isDisabled}
			isRequired={isRequired}
			className={containerClassName}
		>
			<TmobisFormControlLabel label={label} />
			<TmobisSimpleInput
				ref={field?.ref}
				value={value ?? field?.value}
				isInvalid={isInvalid}
				isDisabled={isDisabled}
				isReadonly={isReadonly}
				editable={!isDisabled && !isReadonly}
				onBlur={handleBlur}
				onChangeText={handleChangeText}
				onRemove={onRemove}
				{...props}
			/>
			<TmobisFormControlMessage
				helperText={helperText}
				isInvalid={isInvalid}
				errorText={errorText}
			/>
		</FormControl>
	);
};

export { TmobisInput, TmobisSimpleInput, type TmobisInputProps };


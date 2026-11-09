import {
	getInputFormat,
	getNormalizedBoundaries,
	normalizeDate,
	serializeDate,
	type TmobisDatepickerValue
} from '@tmobis/core/components/datetime/use-datetime';
import { EditFormFieldProps } from '@tmobis/core/components/entity-edit';
import { useEntityEditFieldState } from '@tmobis/core/components/entity-edit/context';
import {
	TmobisFormControlLabel,
	TmobisFormControlMessage
} from '@tmobis/core/components/form-control';
import {
	DateTimePicker,
	DateTimePickerInput,
	DateTimePickerProps,
	DateTimePickerTrigger
} from '@tmobis/core/components/ui/date-time-picker';
import { FormControl } from '@tmobis/core/components/ui/form-control';
import { useCallback } from 'react';

type TmobisSimpleDatetimeProps =
	& Omit<DateTimePickerProps,
		| 'disabled'
		| 'value'
		| 'minimumDate'
		| 'maximumDate'
	> & {
		value?: TmobisDatepickerValue;
		defaultValue?: TmobisDatepickerValue;
		minimumDate?: TmobisDatepickerValue;
		maximumDate?: TmobisDatepickerValue;
		isDisabled?: boolean;
	};

type TmobisDatetimeProps =
	& EditFormFieldProps
	& TmobisSimpleDatetimeProps
	& {
		helperText?: string;
		onChange?: (date: TmobisDatepickerValue) => void;
	};

const TmobisSimpleDatetime = ({
	mode,
	isDisabled,
	value,
	minimumDate,
	maximumDate,
	format,
	onChange,
	placeholder = 'Выберите значение',
	...props
}: TmobisSimpleDatetimeProps & {}) => {
	const {
		normalizedMinimumDate,
		normalizedMaximumDate
	} = getNormalizedBoundaries(
		minimumDate,
		maximumDate
	);
	const resolvedValue = normalizeDate(value);
	const resolvedFormat = getInputFormat(mode, format);

	return (
		<DateTimePicker
			disabled={isDisabled}
			mode={mode}
			value={resolvedValue}
			minimumDate={normalizedMinimumDate}
			maximumDate={normalizedMaximumDate}
			format={resolvedFormat}
			onChange={onChange}
			placeholder={placeholder}
			{...props}
		>
			<DateTimePickerTrigger disabled={isDisabled}>
				<DateTimePickerInput placeholder={placeholder} />
			</DateTimePickerTrigger>
		</DateTimePicker>
	);
};

const TmobisDatetime = ({
	isDisabled,
	fieldName,
	isRequired,
	defaultValue,
	validationRules,
	label,
	helperText,
	onChange,
	mode = 'datetime',
	...props
}: TmobisDatetimeProps) => {
	const { field, errorText, isInvalid } = useEntityEditFieldState({
		fieldName,
		isRequired,
		defaultValue,
		validationRules,
	});
	
	const resolvedValue = normalizeDate(field?.value ?? defaultValue);
	const isDateValue = field?.value instanceof Date || defaultValue instanceof Date;
	const shouldSerializeAsString = !isDateValue;

	const handleChange = useCallback(
		(date: Date | undefined) => {
			const nextValue = serializeDate(
				date ?? null,
				shouldSerializeAsString,
				mode
			);
			onChange?.(nextValue as TmobisDatepickerValue);
			field?.onChange(nextValue ?? undefined);
		},
		[field, mode, onChange, shouldSerializeAsString]
	);

	return (
		<FormControl
			isInvalid={isInvalid}
			isDisabled={isDisabled}
			isRequired={isRequired}
		>
			<TmobisFormControlLabel label={label} />
			<TmobisSimpleDatetime
				value={resolvedValue}
				isDisabled={isDisabled}
				onChange={handleChange}
				mode={mode}
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

export { TmobisDatetime, TmobisSimpleDatetime };

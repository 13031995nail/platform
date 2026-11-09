import { TmobisInput, type TmobisInputProps } from "@tmobis/core/components/input";
import { getNumericKeyboardType, type DefaultRegexType } from './regex-config';
import { useRegexInput } from './use-regex-input';

type TmobisRegexInputProps =
	Omit<TmobisInputProps, 'value' | 'onChangeText' | 'keyboardType'> & {
		regexpr?: string | RegExp;
		defaultRegexType?: DefaultRegexType;
		allowEmpty?: boolean;
		emptyValue?: string | number | null;
		notANumber?: boolean;
		maxlength?: number;
		onChangeText?: (value: string | number | null) => void;
	};

const TmobisRegexInput = ({
	fieldName,
	value,

	regexpr,
	defaultRegexType,
	allowEmpty,
	defaultValue,
	emptyValue,
	notANumber,
	maxlength = -1,

	...props
}: TmobisRegexInputProps) => {
	const {
		settings,
		displayValue,
		handleChangeText,
		handleBlur
	} = useRegexInput({
		regexpr,
		defaultRegexType,
		allowEmpty,
		defaultValue,
		emptyValue,
		notANumber,
		value,
	});

	const keyboardType = getNumericKeyboardType(
		settings?.valueType,
		settings?.isPhone
	);

	return (
		<TmobisInput
			fieldName={fieldName}
			value={displayValue}
			keyboardType={keyboardType}
			onBlur={handleBlur}
			onChangeText={handleChangeText}
			maxLength={maxlength > 0 ? maxlength : undefined}
			{...props}
		/>
	);
};

export default TmobisRegexInput;

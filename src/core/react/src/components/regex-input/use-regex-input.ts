import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
	resolveRegexSettings,
	type DefaultRegexType,
	type RegexInputSettings,
} from './regex-config';
import {
	formatNumericDisplay,
	formatPhoneDisplay,
	isPartialNumericInput,
	parseBoundValue,
	valueToDisplay,
} from './regex-format';

export interface UseRegexInputOptions {
	regexpr?: string | RegExp;
	defaultRegexType?: DefaultRegexType;
	allowEmpty?: boolean;
	defaultValue?: string | number;
	emptyValue?: null | string | number;
	notANumber?: boolean;
	value?: string | number | null;
	onChange?: (value: string | number | null) => void;
}

export function useRegexInput({
	regexpr,
	defaultRegexType,
	allowEmpty,
	defaultValue,
	emptyValue,
	notANumber = false,
	value: controlledValue,
	onChange,
}: UseRegexInputOptions) {
	const regexprKey = regexpr instanceof RegExp ? regexpr.toString() : regexpr;

	const settings = useMemo(
		() =>
			resolveRegexSettings({
				regexpr,
				defaultRegexType,
				allowEmpty,
				defaultValue,
				emptyValue,
			}),
		[regexprKey, defaultRegexType, allowEmpty, defaultValue, emptyValue]
	);

	const lastEmittedRef = useRef<string | number | null | undefined>(controlledValue);
	const [displayText, setDisplayText] = useState(() =>
		valueToDisplay(controlledValue, settings)
	);

	useEffect(() => {
		if (controlledValue === undefined) {
			return;
		}
		if (controlledValue !== lastEmittedRef.current) {
			setDisplayText(valueToDisplay(controlledValue, settings));
			lastEmittedRef.current = controlledValue;
		}
	}, [controlledValue, settings]);

	const commitDisplay = useCallback(
		(text: string, finalize = false) => {
			if (!settings) {
				setDisplayText(text);
				lastEmittedRef.current = text;
				onChange?.(text);
				return;
			}

			let nextDisplay = text;

			if (settings.isPhone && settings.phoneFormat) {
				nextDisplay = formatPhoneDisplay(text, settings.phoneFormat);
			} else if (finalize) {
				nextDisplay = formatNumericDisplay(text, settings, settings.valueType);
			} else if (!isPartialNumericInput(text, settings)) {
				return;
			}

			setDisplayText(nextDisplay);

			const bound = parseBoundValue(
				nextDisplay,
				settings,
				settings.valueType,
				notANumber
			);
			lastEmittedRef.current = bound;
			onChange?.(bound);
		},
		[notANumber, onChange, settings]
	);

	const handleChangeText = useCallback(
		(text: string) => {
			commitDisplay(text);
		},
		[commitDisplay]
	);

	const handleBlur = useCallback(() => {
		if (!settings) {
			return;
		}

		if (settings.isPhone && settings.phoneFormat) {
			if (!settings.pattern.test(displayText)) {
				commitDisplay(settings.phoneFormat);
			}
			return;
		}

		commitDisplay(displayText, true);
	}, [commitDisplay, displayText, settings]);

	return {
		settings,
		displayValue: displayText,
		handleChangeText,
		handleBlur,
	};
}

export type { RegexInputSettings };

export type RegexValueType = 'int' | 'uint' | 'decimal';

export type DefaultRegexType =
	| 'int'
	| 'uint'
	| `uint${number}`
	| 'decimal'
	| `decimal${number}`
	| `decimal${number},${number}`
	| `decimal${number}-${number}`
	| `decimal-fix${number}`
	| `decimal-fix${number},${number}`
	| `decimal-fix${number}-${number}`
	| `udecimal${string}`
	| 'phone'
	| (string & {});

export interface RegexInputSettings {
	pattern: RegExp;
	valueType?: RegexValueType;
	allowNegative: boolean;
	allowEmpty: boolean;
	defaultValue: string | number;
	defaultValueInput: string;
	emptyValue: null | string | number;
	beforePoint?: number;
	afterPoint?: number;
	afterPointFrom?: number;
	fixatingFloatPoint?: boolean;
	decimalMark?: string[];
	numericType?: 'int' | 'decimal';
	isPhone?: boolean;
	phoneFormat?: string;
}

const DECIMAL_MARKS = ['.', ','] as const;

function getDecimalMarksString(decimalMark: string[]): string {
	return `(${decimalMark.map((m) => `\\${m}`).join('|')})`;
}

type NumericSettingsFields = Required<
	Pick<
		RegexInputSettings,
		'beforePoint' | 'afterPoint' | 'fixatingFloatPoint' | 'decimalMark' | 'numericType'
	>
> &
	Pick<RegexInputSettings, 'afterPointFrom'>;

function numericSettingsFields(params: {
	beforePoint: number;
	afterPoint: number;
	afterPointFrom?: number;
	fixatingFloatPoint: boolean;
	numericType: 'int' | 'decimal';
}): NumericSettingsFields {
	return {
		beforePoint: params.beforePoint,
		afterPoint: params.afterPoint,
		afterPointFrom: params.afterPointFrom,
		fixatingFloatPoint: params.fixatingFloatPoint,
		decimalMark: [...DECIMAL_MARKS],
		numericType: params.numericType,
	};
}

function buildNumericPattern(settings: Pick<
	RegexInputSettings,
	'allowNegative' | 'beforePoint' | 'afterPoint' | 'numericType' | 'decimalMark'
> & {
	beforePoint: number;
	afterPoint: number;
	decimalMark: string[];
}): RegExp {
	const negPart = settings.allowNegative ? '[-]?' : '';
	const intPart = `^${negPart}\\d{1,${settings.beforePoint}}$`;
	const decimalPart =
		`^${negPart}\\d{1,${settings.beforePoint}}` +
		`${getDecimalMarksString(settings.decimalMark)}\\d{0,${settings.afterPoint}}$`;

	if (settings.numericType === 'int') {
		return new RegExp(intPart);
	}
	return new RegExp(`${intPart}|${decimalPart}`);
}

function parseAfterPointCounts(counts: string[]): {
	beforePoint: number;
	afterPoint: number;
	afterPointFrom?: number;
	numericType: 'decimal';
} {
	const parseFromTo = (fromToStr: string) => {
		const parts = fromToStr.split('-');
		switch (parts.length) {
			case 1:
				return { afterPoint: parseInt(parts[0], 10), afterPointFrom: undefined };
			case 2:
				return {
					afterPoint: parseInt(parts[1], 10),
					afterPointFrom: parseInt(parts[0], 10),
				};
			default:
				throw new Error('Некорректный тип');
		}
	};

	if (counts.length === 1) {
		const { afterPoint, afterPointFrom } = parseFromTo(counts[0]);
		return { beforePoint: 20, afterPoint, afterPointFrom, numericType: 'decimal' };
	}
	if (counts.length === 2) {
		const { afterPoint, afterPointFrom } = parseFromTo(counts[1]);
		const beforePoint = parseInt(counts[0], 10) - afterPoint;
		return { beforePoint, afterPoint, afterPointFrom, numericType: 'decimal' };
	}
	throw new Error('Некорректный тип');
}

export function resolveRegexSettings(options: {
	regexpr?: string | RegExp;
	defaultRegexType?: string;
	allowEmpty?: boolean;
	defaultValue?: string | number;
	emptyValue?: null | string | number;
}): RegexInputSettings | null {
	const allowEmpty = options.allowEmpty ?? true;
	const defaultValue =
		options.defaultValue !== undefined && options.defaultValue !== null
			? options.defaultValue
			: '';
	const defaultValueInput = String(defaultValue);
	const emptyValue = options.emptyValue ?? null;

	let regexpr = options.regexpr;
	let defaultRegexType = options.defaultRegexType;
	let allowNegative = true;
	let beforePoint = 20;
	let afterPoint = 0;
	let afterPointFrom: number | undefined;
	let fixatingFloatPoint = false;
	let valueType: RegexValueType | undefined;
	let numericType: 'int' | 'decimal' | undefined;
	let pattern: RegExp | undefined;

	if (regexpr == null && defaultRegexType == null) {
		return null;
	}

	if (regexpr == null && defaultRegexType != null) {
		if (defaultRegexType.substring(0, 8) === 'udecimal') {
			allowNegative = false;
			defaultRegexType = defaultRegexType.substring(1);
		}

		if (defaultRegexType.substring(0, 7) === 'decimal') {
			if (defaultRegexType.length === 7) {
				regexpr = allowNegative
					? '^(?:-?(?:(?:0|[1-9]\\d*)(?:[.,]\\d*)?)?)$'
					: '^(?:(?:0|[1-9]\\d*)(?:[.,]\\d*)?)$';
			} else {
				fixatingFloatPoint =
					defaultRegexType.length >= 11 &&
					defaultRegexType.substring(0, 11) === 'decimal-fix';
				const suffix = defaultRegexType.substring(fixatingFloatPoint ? 11 : 7);
				try {
					const parsed = parseAfterPointCounts(suffix.split(','));
					beforePoint = parsed.beforePoint;
					afterPoint = parsed.afterPoint;
					afterPointFrom = parsed.afterPointFrom;
					numericType = parsed.numericType;
					valueType = 'decimal';
				} catch {
					console.warn(
						`Ошибка при получении значения типа маски regex-input. Некорректный тип: ${defaultRegexType}`
					);
				}
			}
		} else if (defaultRegexType === 'int') {
			valueType = 'int';
			numericType = 'int';
		} else if (defaultRegexType.substring(0, 4) === 'uint') {
			valueType = 'uint';
			numericType = 'int';
			allowNegative = false;
			const counts = defaultRegexType.substring(4);
			if (counts.length > 0) {
				const parsed = parseInt(counts, 10);
				if (!Number.isNaN(parsed)) {
					beforePoint = parsed;
				} else {
					console.warn(
						`Ошибка при получении значения типа маски regex-input. Некорректный тип: ${defaultRegexType}`
					);
				}
			}
		} else if (defaultRegexType === 'phone') {
			const phoneFormat = '+7 ___ ___-__-__';
			return {
				pattern: /^\+?(7|8)[\s]((?:\d|_){3})[\s]((?:\d|_){3})[-]((?:\d|_){2})[-]((?:\d|_){2})$/,
				allowNegative,
				allowEmpty,
				defaultValue: defaultValueInput ? defaultValue : phoneFormat,
				defaultValueInput: defaultValueInput || phoneFormat,
				emptyValue,
				isPhone: true,
				phoneFormat,
			};
		} else {
			console.warn(
				`Ошибка при получении значения типа маски regex-input. Некорректный тип: ${defaultRegexType}`
			);
		}
	}

	if (pattern == null) {
		if (regexpr instanceof RegExp) {
			pattern = regexpr;
		} else if (typeof regexpr === 'string') {
			pattern = new RegExp(regexpr);
		} else if (numericType != null) {
			pattern = buildNumericPattern({
				allowNegative,
				beforePoint,
				afterPoint,
				numericType,
				decimalMark: [...DECIMAL_MARKS],
			});
		} else {
			return null;
		}
	}

	const commonFields = {
		pattern,
		valueType,
		allowNegative,
		allowEmpty,
		defaultValue,
		defaultValueInput,
		emptyValue,
	};

	if (numericType != null) {
		return {
			...commonFields,
			...numericSettingsFields({
				beforePoint,
				afterPoint,
				afterPointFrom,
				fixatingFloatPoint,
				numericType,
			}),
		};
	}

	return commonFields;
}

export function getNumericKeyboardType(
	valueType?: RegexValueType,
	isPhone?: boolean
): 'default' | 'numeric' | 'decimal-pad' | 'phone-pad' {
	if (isPhone) {
		return 'phone-pad';
	}
	if (valueType === 'int' || valueType === 'uint') {
		return 'numeric';
	}
	if (valueType === 'decimal') {
		return 'decimal-pad';
	}
	return 'default';
}

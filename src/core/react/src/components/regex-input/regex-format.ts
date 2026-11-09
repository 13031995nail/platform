import type { RegexInputSettings, RegexValueType } from './regex-config';

function findDecimalMarkIndex(value: string, decimalMark: string[]): number {
	for (const mark of decimalMark) {
		const index = value.indexOf(mark);
		if (index !== -1) {
			return index;
		}
	}
	return -1;
}

export function formatNumericDisplay(
	value: string | undefined,
	settings: RegexInputSettings,
	valueType?: RegexValueType
): string {
	const val = value ?? '';
	if (!val) {
		return settings.allowEmpty ? val : settings.defaultValueInput;
	}

	if (!settings.pattern.test(val) && !settings.isPhone) {
		return settings.defaultValueInput;
	}

	if (
		settings.fixatingFloatPoint &&
		settings.afterPoint != null &&
		settings.afterPoint > 0 &&
		settings.decimalMark?.length
	) {
		let newValue = val;
		const decimalMark = settings.decimalMark;
		const pointIndex = findDecimalMarkIndex(newValue, decimalMark);
		let addingZerosCount = settings.afterPointFrom ?? settings.afterPoint;

		if (pointIndex === -1) {
			newValue += decimalMark[0];
		} else {
			const parts = newValue.split(decimalMark.find((m) => newValue.includes(m))!);
			if (parts.length > 2) {
				addingZerosCount = 0;
			} else if (parts.length === 2) {
				addingZerosCount -= parts[1].length;
			}
		}

		if (addingZerosCount >= 0) {
			newValue += '0'.repeat(addingZerosCount);
		} else {
			const newLength =
				newValue.length +
				addingZerosCount +
				(settings.afterPointFrom === undefined
					? 0
					: settings.afterPoint - settings.afterPointFrom);
			newValue = newValue.substring(0, newLength);
		}

		return newValue;
	}

	if (valueType === 'int' || valueType === 'uint') {
		const parsed = parseInt(val, 10);
		if (Number.isNaN(parsed)) {
			return settings.defaultValueInput;
		}
		const normalized = valueType === 'uint' && parsed < 0 ? Math.abs(parsed) : parsed;
		return `${normalized}`;
	}

	return val;
}

function buildPhoneFromDigits(digits: string): string {
	let national = digits;
	if (national.startsWith('8')) {
		national = national.slice(1);
	} else if (national.startsWith('7')) {
		national = national.slice(1);
	}
	national = national.slice(0, 10);

	const group1 = (national.slice(0, 3) + '___').slice(0, 3);
	const group2 = (national.slice(3, 6) + '___').slice(0, 3);
	const group3 = (national.slice(6, 8) + '__').slice(0, 2);
	const group4 = (national.slice(8, 10) + '__').slice(0, 2);

	return `+7 ${group1} ${group2}-${group3}-${group4}`;
}

export function formatPhoneDisplay(value: string, phoneFormat: string): string {
	const digitsOnly = value.replace(/\D/g, '');
	if (digitsOnly.length === 0) {
		return phoneFormat;
	}
	return buildPhoneFromDigits(digitsOnly);
}

export function isPartialNumericInput(
	text: string,
	settings: RegexInputSettings
): boolean {
	if (text === '' && settings.allowEmpty) {
		return true;
	}
	if (settings.allowNegative && text === '-') {
		return true;
	}
	return settings.pattern.test(text);
}

export function parseBoundValue(
	displayValue: string,
	settings: RegexInputSettings,
	valueType: RegexValueType | undefined,
	notANumber: boolean
): string | number | null {
	if (!displayValue) {
		return settings.allowEmpty ? settings.emptyValue : settings.defaultValue;
	}

	if (settings.isPhone && settings.phoneFormat) {
		if (displayValue === settings.phoneFormat) {
			return settings.allowEmpty ? settings.emptyValue : settings.defaultValue;
		}
		return displayValue;
	}

	switch (valueType) {
		case 'int':
		case 'uint':
			if (notANumber) {
				return displayValue;
			}
			return parseInt(displayValue, 10);
		case 'decimal':
			return parseFloat(displayValue.replace(',', '.'));
		default:
			return displayValue;
	}
}

export function valueToDisplay(
	value: string | number | null | undefined,
	settings: RegexInputSettings | null,
	finalize = true
): string {
	if (value === null || value === undefined || value === '') {
		if (settings && !settings.allowEmpty) {
			return settings.isPhone && settings.phoneFormat
				? settings.phoneFormat
				: settings.defaultValueInput;
		}
		return '';
	}

	const str = String(value);
	if (!settings) {
		return str;
	}

	if (settings.isPhone && settings.phoneFormat) {
		return formatPhoneDisplay(str, settings.phoneFormat);
	}

	if (finalize) {
		return formatNumericDisplay(str, settings, settings.valueType);
	}

	return str;
}

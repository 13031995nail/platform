'use client';

import { DateTimePickerMode } from '@tmobis/core/components/ui/date-time-picker';
import dayjs from '@tmobis/core/initializers/dayjs';

export type TmobisDatepickerValue = Date | string | undefined;

const DEFAULT_DATE_FORMAT = 'YYYY-MM-DD';
const DEFAULT_DATE_TIME_FORMAT = 'YYYY-MM-DD HH:mm';
const DEFAULT_TIME_FORMAT = 'HH:mm';
const SUPPORTED_VALUE_FORMATS = [
	DEFAULT_DATE_FORMAT,
	DEFAULT_DATE_TIME_FORMAT,
	DEFAULT_TIME_FORMAT,
	'HH:mm:ss',
];

export const normalizeDate = (value: TmobisDatepickerValue): Date | undefined => {
	if (!value) {
		return undefined;
	}

	if (value instanceof Date) {
		return Number.isNaN(value.getTime()) ? undefined : value;
	}

	const parsedValue = dayjs(value, SUPPORTED_VALUE_FORMATS, true);
	const date = parsedValue.isValid() ? parsedValue : dayjs(value);

	return date.isValid() ? date.toDate() : undefined;
};

export const serializeDate = (
	date: Date | null,
	shouldSerializeAsString: boolean,
	mode: DateTimePickerMode,
): TmobisDatepickerValue => {
	if (!date) {
		return undefined;
	}

	if (!shouldSerializeAsString) {
		return date;
	}

	if (mode === 'time') {
		return dayjs(date).format(DEFAULT_TIME_FORMAT);
	}

	if (mode === 'date') {
		return dayjs(date).format(DEFAULT_DATE_FORMAT);
	}

	return date.toISOString();
};

export const getInputFormat = (mode?: DateTimePickerMode, format?: string): string => {
	return (
		format ??
		(mode === 'time'
			? DEFAULT_TIME_FORMAT
			: mode === 'datetime'
				? DEFAULT_DATE_TIME_FORMAT
				: DEFAULT_DATE_FORMAT
		)
	);
};

export const getNormalizedBoundaries = (
	minimumDate?: TmobisDatepickerValue,
	maximumDate?: TmobisDatepickerValue,
) => {
	return {
		normalizedMinimumDate: normalizeDate(minimumDate),
		normalizedMaximumDate: normalizeDate(maximumDate),
	};
};

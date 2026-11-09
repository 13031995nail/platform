import dayjs from '../../initializers/dayjs';
import { FieldPath, FieldValues } from 'react-hook-form';

type ValidationResult = boolean | string | Promise<boolean | string>;
export type EditFormValidator = (
	value: unknown,
	formValues: FieldValues
) => ValidationResult;

const isEmptyValue = (value: unknown) =>
	value === undefined || value === null || value === '';

const getPathValue = (formValues: FieldValues, fieldName: string) =>
	fieldName
		.split('.')
		.reduce<unknown>(
			(value, key) =>
				value && typeof value === 'object'
					? (value as Record<string, unknown>)[key]
					: undefined,
			formValues
		);

const toDate = (value: unknown) => {
	if (value instanceof Date) {
		return Number.isNaN(value.getTime()) ? undefined : value;
	}

	if (typeof value === 'string') {
		const date = dayjs(value, 'YYYY-MM-DD', true);
		return date.isValid() ? date.toDate() : undefined;
	}

	return undefined;
};

export const requiredWhen =
	<TFieldValues extends FieldValues = FieldValues>(
		predicate: (formValues: TFieldValues) => boolean,
		message = 'Поле обязательно для заполнения'
	): EditFormValidator =>
		(value, formValues) => {
			if (!predicate(formValues as TFieldValues)) {
				return true;
			}

			return !isEmptyValue(value) || message;
		};

export const dateAfter =
	<TFieldValues extends FieldValues = FieldValues>(
		fieldName: FieldPath<TFieldValues>,
		message = 'Дата должна быть больше'
	): EditFormValidator =>
		(value, formValues) => {
			if (isEmptyValue(value)) {
				return true;
			}

			const date = toDate(value);
			if (!date) {
				return 'Некорректная дата';
			}

			const anotherValue = getPathValue(formValues, fieldName);
			if (isEmptyValue(anotherValue)) {
				return true;
			}

			const anotherDate = toDate(anotherValue);
			if (!anotherDate) {
				return 'Некорректная дата';
			}

			return date > anotherDate || message;
		};

export const dateBefore =
	<TFieldValues extends FieldValues = FieldValues>(
		fieldName: FieldPath<TFieldValues>,
		message = 'Дата должна быть меньше'
	): EditFormValidator =>
		(value, formValues) => {
			if (isEmptyValue(value)) {
				return true;
			}

			const date = toDate(value);
			if (!date) {
				return 'Некорректная дата';
			}

			const anotherValue = getPathValue(formValues, fieldName);
			if (isEmptyValue(anotherValue)) {
				return true;
			}

			const anotherDate = toDate(anotherValue);
			if (!anotherDate) {
				return 'Некорректная дата';
			}

			return date < anotherDate || message;
		};

export const isEmail =
	(message = 'Введите корректный email'): EditFormValidator =>
		(value) => {
			if (isEmptyValue(value)) {
				return true;
			}

			return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(value)) || message;
		};

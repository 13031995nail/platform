import dayjs from 'dayjs';

const isDateRegex = /^\d{4}-\d{2}-\d{2}(T.*)?$/;

export function isDateValue(value: unknown): boolean {
	return value instanceof Date || (typeof value === 'string' && isDateRegex.test(value));
}

export function formatDate(
	value: string | Date | DateOnly | null | undefined,
	dateFormat: string = 'DD.MM.YYYY'
): string {
	const dateValue = value instanceof DateOnly ? value.toDate() : value;

	if (isDateValue(dateValue)) {
		return dayjs(dateValue).format(dateFormat);
	}

	return dateValue?.toString() ?? '';
}

export function zeroTimeData(value: string | Date | DateOnly): Date {
	const dateEmptyTime = new DateOnly(value);

	return dateEmptyTime.toDate();
}

export const getTimestamp = (date: DateOnly | Date | string | null | undefined, nullFirst: boolean = false): number => {
	const infinity = nullFirst ? +Infinity : -Infinity;

	if (!date) {
		return infinity;
	}

	const dateValue = date instanceof DateOnly ? date.toDate() : date;
	const time = new Date(dateValue).getTime();

	return isNaN(time) ? infinity : time;
};

/**
 * Класс для работы с датой.
 */
export class DateOnly {
	private readonly _date: Date;

	/**
	 * Конструктор класса DateOnly.
	 * @param value Значение для инициализации.
	 * @default Date.
	 */
	constructor(value?: string | number | Date | DateOnly | null) {
		if (value instanceof DateOnly) {
			this._date = value.toDate();
		} else {
			this._date = new Date(value ?? Date.now());
		}

		this._date.setHours(0, 0, 0, 0);
	}

	/**
	 * Сериализует дату в строку формата ISO.
	 * @returns Строка даты в формате ISO.
	 */
	public toJSON(): string {
		return this.toISOString();
	}

	/**
	 * Возвращает строковое представление даты.
	 * @returns Строковое представление даты.
	 */
	public toString(): string {
		return this._date.toString();
	}

	/**
	 * Возвращает дату в формате ISO.
	 * @returns Строка даты в формате ISO.
	 */
	public toISOString(): string {
		return dayjs(this._date).format('YYYY-MM-DD');
	}

	/**
	 * Возвращает объект Date с обнуленным временем.
	 * @returns Новый объект `Date`.
	 */
	public toDate(): Date {
		return new Date(this._date);
	}
}

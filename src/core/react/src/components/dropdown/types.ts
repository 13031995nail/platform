import type { IEntity } from '../../models/entity';
import type { EditFormFieldProps } from '../entity-edit';

export interface DropdownItem<T extends IEntity = IEntity> {
	value: string;
	label: string;
	object?: T | null;
}

export interface DropdownBaseProps {
	placeholder?: string;
	isInvalid?: boolean;
	isDisabled?: boolean;
	error?: boolean;
	disabled?: boolean;
	hiddenClearIcon?: boolean;
	searchable?: boolean;
	/**
	 * Расположение поля поиска: в триггере или в выпадающей панели.
	 */
	searchPlacement?: 'trigger' | 'panel';
	/**
	 * Минимальная длина строки для запуска поиска.
	 */
	minSearchLength?: number;
	/**
	 * Задержка debounce для серверного поиска (мс).
	 */
	searchDebounceMs?: number;
	/**
	 * Placeholder поля поиска в панели.
	 */
	searchPlaceholder?: string;
	onSearch?: (value: string | null) => void;
	loading?: boolean;
	hasNext?: boolean;
	onOpen?: () => void;
	onOpenChange?: (isOpen: boolean) => void;
	onScrollEnd?: () => void;
	/**
	 * Форматирование текста выбранных значений в поле.
	 */
	formatSelectedLabel?: (
		items: Array<{ value: string; label: string }>
	) => string;
}

export interface DropdownEnumSingleProps<E extends Record<string, string>>
	extends DropdownBaseProps {
	options: E;
	value: E[keyof E] | null;
	onChange: (value: E[keyof E] | null) => void;
	multiple?: false;
}

export interface DropdownEnumMultiProps<E extends Record<string, string>>
	extends DropdownBaseProps {
	options: E;
	value: E[keyof E][] | null;
	onChange: (value: E[keyof E][] | null) => void;
	multiple: true;
}

export interface DropdownArraySingleProps<T extends IEntity>
	extends DropdownBaseProps {
	options: T[];
	value: string | null;
	defaultValue?: string;
	onChange: (value: string | null, option: T | null) => void;
	getOptionLabel: (option: T) => string;
	getOptionDisabled?: (option: DropdownItem<T>) => boolean;
	multiple?: false;
}

export interface DropdownArrayMultiProps<T extends IEntity>
	extends DropdownBaseProps {
	options: T[];
	value: string[] | null;
	onChange: (value: string[] | null, options: T[]) => void;
	getOptionLabel: (option: T) => string;
	getOptionDisabled?: (option: DropdownItem<T>) => boolean;
	multiple: true;
}

export type TmobisSimpleDropdownProps<
	E extends Record<string, string>,
	T extends IEntity,
> =
	| DropdownEnumSingleProps<E>
	| DropdownEnumMultiProps<E>
	| DropdownArraySingleProps<T>
	| DropdownArrayMultiProps<T>;

/** @deprecated Используйте TmobisSimpleDropdownProps */
export type DropdownControlProps<
	E extends Record<string, string>,
	T extends IEntity,
> = TmobisSimpleDropdownProps<E, T>;

type ValueOnChangeEnumSingle<E extends Record<string, string>> = {
	value?: DropdownEnumSingleProps<E>['value'];
	onChange?: DropdownEnumSingleProps<E>['onChange'];
};

type ValueOnChangeEnumMulti<E extends Record<string, string>> = {
	value?: DropdownEnumMultiProps<E>['value'];
	onChange?: DropdownEnumMultiProps<E>['onChange'];
};

type ValueOnChangeArraySingle<T extends IEntity> = {
	value?: DropdownArraySingleProps<T>['value'];
	onChange?: DropdownArraySingleProps<T>['onChange'];
};

type ValueOnChangeArrayMulti<T extends IEntity> = {
	value?: DropdownArrayMultiProps<T>['value'];
	onChange?: DropdownArrayMultiProps<T>['onChange'];
};

export type DropdownEnumFormProps<E extends Record<string, string>> =
	EditFormFieldProps &
	Omit<
		DropdownEnumSingleProps<E>,
		'value' | 'onChange' | 'isInvalid' | 'error'
	> &
	ValueOnChangeEnumSingle<E> & {
		helperText?: string;
	};

export type DropdownEnumMultiFormProps<E extends Record<string, string>> =
	EditFormFieldProps &
	Omit<
		DropdownEnumMultiProps<E>,
		'value' | 'onChange' | 'isInvalid' | 'error'
	> &
	ValueOnChangeEnumMulti<E> & {
		helperText?: string;
	};

export type DropdownArrayFormProps<T extends IEntity> =
	EditFormFieldProps &
	Omit<
		DropdownArraySingleProps<T>,
		'value' | 'onChange' | 'isInvalid' | 'error'
	> &
	ValueOnChangeArraySingle<T> & {
		helperText?: string;
	};

export type DropdownArrayMultiFormProps<T extends IEntity> =
	EditFormFieldProps &
	Omit<
		DropdownArrayMultiProps<T>,
		'value' | 'onChange' | 'isInvalid' | 'error'
	> &
	ValueOnChangeArrayMulti<T> & {
		helperText?: string;
	};

export type TmobisDropdownProps<
	T extends IEntity,
	E extends Record<string, string>,
> =
	| DropdownEnumFormProps<E>
	| DropdownEnumMultiFormProps<E>
	| DropdownArrayFormProps<T>
	| DropdownArrayMultiFormProps<T>;

export type DropdownProps<
	T extends IEntity,
	E extends Record<string, string>,
> = TmobisDropdownProps<T, E>;

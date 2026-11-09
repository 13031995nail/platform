'use client';

import { TmobisSimpleCheckbox } from '@tmobis/core/components/checkbox';
import { TmobisSimpleInput } from '@tmobis/core/components/input';
import {
	Actionsheet,
	ActionsheetBackdrop,
	ActionsheetContent,
	ActionsheetDragIndicator,
	ActionsheetDragIndicatorWrapper,
	ActionsheetItem,
	ActionsheetItemText,
	ActionsheetScrollView,
} from '@tmobis/core/components/ui/actionsheet';
import {
	DropdownBackdrop,
	DropdownEmpty,
	DropdownItemContent,
	DropdownItemText,
	DropdownLoading,
	DropdownOverlay,
	DropdownPanel,
	DropdownPanelItem,
	DropdownPanelScroll,
	dropdownItemTextStyle,
	dropdownSheetItemStyle,
} from '@tmobis/core/components/ui/dropdown';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { Spinner } from '@tmobis/core/components/ui/spinner';
import { debounce } from '@tmobis/core/utils/debounce';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
	Platform,
	View,
	type NativeScrollEvent,
	type NativeSyntheticEvent,
	type LayoutRectangle,
} from 'react-native';
import type { IEntity } from '../../models/entity';
import type {
	DropdownArrayMultiProps,
	DropdownArraySingleProps,
	DropdownEnumMultiProps,
	DropdownEnumSingleProps,
	DropdownItem,
	TmobisSimpleDropdownProps,
} from './types';
import { useDropdown } from './use-dropdown';
import { rememberSelectedLabels } from './selected-labels';

const SEARCH_DEBOUNCE_MS = 1000;
const NO_OPTIONS_TEXT = 'Нет значений';
const DEFAULT_SEARCH_PLACEHOLDER = 'Поиск...';

function useDebouncedSearch(
	onSearch?: (value: string | null) => void,
	delayMs = SEARCH_DEBOUNCE_MS,
	minSearchLength = 0
) {
	const debouncedRef = useRef<ReturnType<typeof debounce<[string]>> | null>(null);

	useEffect(() => {
		if (!onSearch) {
			debouncedRef.current = null;
			return;
		}
		const debounced = debounce((value: string) => {
			const trimmed = value.trim();
			if (!trimmed || trimmed.length < minSearchLength) {
				onSearch(null);
				return;
			}
			onSearch(trimmed);
		}, delayMs);
		debouncedRef.current = debounced;
		return () => {
			debounced.cancel();
			debouncedRef.current = null;
		};
	}, [delayMs, minSearchLength, onSearch]);

	const runSearch = useCallback((value: string) => {
		debouncedRef.current?.(value);
	}, []);

	const cancelSearch = useCallback(() => {
		debouncedRef.current?.cancel();
	}, []);

	return { runSearch, cancelSearch };
}

type DropdownListProps<T extends IEntity> = {
	items: DropdownItem<T>[];
	multiple: boolean;
	isSelected: (value: string) => boolean;
	onSelect: (item: DropdownItem<T>) => void;
	getOptionDisabled?: (option: DropdownItem<T>) => boolean;
	onScrollEnd?: () => void;
	hasNext?: boolean;
	loading?: boolean;
	variant: 'panel' | 'sheet';
	hideSheetChrome?: boolean;
};

function DropdownList<T extends IEntity>({
	items,
	multiple,
	isSelected,
	onSelect,
	getOptionDisabled,
	onScrollEnd,
	hasNext,
	loading,
	variant,
	hideSheetChrome = false,
}: DropdownListProps<T>) {
	const handleScroll = useCallback(
		(event: NativeSyntheticEvent<NativeScrollEvent>) => {
			if (!onScrollEnd || loading || !hasNext) {
				return;
			}
			const { layoutMeasurement, contentOffset, contentSize } = event.nativeEvent;
			const isEnd =
				contentSize.height - contentOffset.y <= layoutMeasurement.height + 50;
			if (isEnd) {
				onScrollEnd();
			}
		},
		[hasNext, loading, onScrollEnd]
	);

	if (items.length === 0) {
		return <DropdownEmpty>{NO_OPTIONS_TEXT}</DropdownEmpty>;
	}

	const renderItem = (item: DropdownItem<T>) => {
		const selected = isSelected(item.value);
		const disabled = getOptionDisabled?.(item) ?? false;

		if (variant === 'panel') {
			return (
				<DropdownPanelItem
					key={item.value}
					disabled={disabled}
					selected={selected}
					onPress={() => onSelect(item)}
				>
					{multiple ? (
						<DropdownItemContent variant="panel">
							<TmobisSimpleCheckbox
								value={item.value}
								isChecked={selected}
								isDisabled={disabled}
								aria-label={item.label}
							/>
							<DropdownItemText selected={selected}>
								{item.label}
							</DropdownItemText>
						</DropdownItemContent>
					) : (
						<DropdownItemText selected={selected}>
							{item.label}
						</DropdownItemText>
					)}
				</DropdownPanelItem>
			);
		}

		return (
			<ActionsheetItem
				key={item.value}
				disabled={disabled}
				onPress={() => onSelect(item)}
				className={dropdownSheetItemStyle({ selected })}
			>
				{multiple ? (
					<DropdownItemContent variant="sheet">
						<TmobisSimpleCheckbox
							value={item.value}
							isChecked={selected}
							isDisabled={disabled}
							aria-label={item.label}
						/>
						<ActionsheetItemText
							className={dropdownItemTextStyle({ selected })}
						>
							{item.label}
						</ActionsheetItemText>
					</DropdownItemContent>
				) : (
					<ActionsheetItemText
						className={dropdownItemTextStyle({ selected })}
					>
						{item.label}
					</ActionsheetItemText>
				)}
			</ActionsheetItem>
		);
	};

	if (variant === 'panel') {
		return <>{items.map(renderItem)}</>;
	}

	return (
		<ActionsheetScrollView onScroll={handleScroll} scrollEventThrottle={16}>
			{!hideSheetChrome ? (
				<ActionsheetDragIndicatorWrapper>
					<ActionsheetDragIndicator />
				</ActionsheetDragIndicatorWrapper>
			) : null}
			{items.map(renderItem)}
			{loading ? (
				<DropdownLoading size="sheet">
					<Spinner size="small" />
				</DropdownLoading>
			) : null}
		</ActionsheetScrollView>
	);
}

function TmobisSimpleDropdown<
	E extends Record<string, string>,
	T extends IEntity,
>(props: TmobisSimpleDropdownProps<E, T>) {
	const isEnum = !Array.isArray((props as DropdownArraySingleProps<T>).options);
	const multiple = props.multiple === true;
	const isCombobox =
		props.searchable === true || typeof props.onSearch === 'function';
	const searchPlacement = props.searchPlacement ?? 'trigger';
	const isPanelSearch = isCombobox && searchPlacement === 'panel';
	const isTriggerCombobox = isCombobox && !isPanelSearch;
	const minSearchLength = props.minSearchLength ?? 0;
	const searchDebounceMs = props.searchDebounceMs ?? SEARCH_DEBOUNCE_MS;
	const searchPlaceholder = props.searchPlaceholder ?? DEFAULT_SEARCH_PLACEHOLDER;

	const {
		options,
		placeholder = 'Выберите значение',
		isInvalid: isInvalidProp,
		isDisabled: isDisabledProp,
		error,
		disabled,
		hiddenClearIcon,
		loading,
		hasNext,
		onOpen,
		onOpenChange,
		onScrollEnd,
		onSearch,
		formatSelectedLabel,
	} = props;

	const isInvalid = isInvalidProp ?? error ?? false;
	const isDisabled = isDisabledProp ?? disabled ?? false;

	const getOptionLabel = !isEnum
		? (props as DropdownArraySingleProps<T>).getOptionLabel
		: undefined;
	const getOptionDisabled = !isEnum
		? (props as DropdownArraySingleProps<T>).getOptionDisabled
		: undefined;
	const defaultValue = !isEnum
		? (props as DropdownArraySingleProps<T>).defaultValue
		: undefined;

	const { items, itemsMap } = useDropdown(
		options as Record<string, string> | T[],
		getOptionLabel
	);

	const [isOpen, setIsOpen] = useState(false);
	const [query, setQuery] = useState('');
	const [anchor, setAnchor] = useState<LayoutRectangle | null>(null);
	const anchorRef = useRef<View>(null);
	const { runSearch, cancelSearch } = useDebouncedSearch(
		onSearch,
		searchDebounceMs,
		minSearchLength
	);

	const measureAnchor = useCallback(() => {
		anchorRef.current?.measureInWindow((x, y, width, height) => {
			setAnchor({ x, y, width, height });
		});
	}, []);

	const value = props.value;
	const onChange = props.onChange;

	const selectedValues = useMemo<string[]>(() => {
		if (!multiple) {
			const single = value as string | null;
			return single ? [single] : [];
		}
		return (value as string[] | null) ?? [];
	}, [multiple, value]);

	// Метки выбранных значений живут отдельно от текущих options:
	// серверная выдача может больше не содержать выбранный объект.
	const [knownLabels, setKnownLabels] = useState<ReadonlyMap<string, string>>(
		() => new Map()
	);
	useEffect(() => {
		setKnownLabels((previous) => {
			const next = rememberSelectedLabels(previous, selectedValues, items);
			if (
				next.size === previous.size &&
				Array.from(next).every(([id, label]) => previous.get(id) === label)
			) {
				return previous;
			}
			return next;
		});
	}, [items, selectedValues]);

	const selectedItems = useMemo(() => {
		return selectedValues
			.map((id) => itemsMap[id] ?? (
				knownLabels.has(id)
					? { value: id, label: knownLabels.get(id)! }
					: undefined
			))
			.filter((item): item is DropdownItem<T> => !!item);
	}, [itemsMap, knownLabels, selectedValues]);

	const displayText = useMemo(() => {
		if (formatSelectedLabel) {
			const formatted = formatSelectedLabel(
				selectedItems.map((item) => ({
					value: item.value,
					label: item.label,
				}))
			);
			if (formatted || selectedValues.length === 0) {
				return formatted;
			}
			// Кастомный formatter может не найти значение в текущей выдаче.
			// В таком случае используем сохранённую метку / defaultValue.
		}

		if (multiple) {
			if (selectedItems.length === 0) {
				return '';
			}
			if (selectedItems.length === 1) {
				return selectedItems[0].label;
			}
			return `Выбрано: ${selectedItems.length}`;
		}

		const singleValue = value as string | null;
		if (!singleValue) {
			return '';
		}
		const item = itemsMap[singleValue];
		if (item) {
			return item.label;
		}
		return knownLabels.get(singleValue) ?? defaultValue ?? '';
	}, [
		knownLabels,
		selectedValues,
		defaultValue,
		formatSelectedLabel,
		itemsMap,
		multiple,
		selectedItems,
		value,
	]);

	const filteredItems = useMemo(() => {
		if (!isCombobox || onSearch) {
			return items;
		}
		const q = query.trim().toLowerCase();
		if (!q || q.length < minSearchLength) {
			return items;
		}
		return items.filter((item) => item.label.toLowerCase().includes(q));
	}, [isCombobox, items, minSearchLength, onSearch, query]);

	useEffect(() => {
		if (!isOpen) {
			setAnchor(null);
			return;
		}

		// Измерение после layout
		const frame = requestAnimationFrame(() => measureAnchor());

		if (Platform.OS !== 'web' || typeof window === 'undefined') {
			return () => cancelAnimationFrame(frame);
		}

		const handleReposition = () => measureAnchor();
		window.addEventListener('resize', handleReposition);
		window.addEventListener('scroll', handleReposition, true);
		return () => {
			cancelAnimationFrame(frame);
			window.removeEventListener('resize', handleReposition);
			window.removeEventListener('scroll', handleReposition, true);
		};
	}, [isOpen, measureAnchor, filteredItems.length, loading]);

	const isSelected = useCallback(
		(itemValue: string) => selectedValues.includes(itemValue),
		[selectedValues]
	);

	const handleOpen = useCallback(() => {
		if (isDisabled) {
			return;
		}
		onOpen?.();
		setIsOpen(true);
		onOpenChange?.(true);
	}, [isDisabled, onOpen, onOpenChange]);

	const resetSearch = useCallback(() => {
		setQuery('');
		cancelSearch();
		onSearch?.(null);
	}, [cancelSearch, onSearch]);

	const handleClose = useCallback(() => {
		setIsOpen(false);
		onOpenChange?.(false);
		if (isCombobox) {
			resetSearch();
		}
	}, [isCombobox, onOpenChange, resetSearch]);

	const emitSingleChange = useCallback(
		(item: DropdownItem<T> | null) => {
			if (isEnum) {
				(onChange as DropdownEnumSingleProps<E>['onChange'])(
					item ? (item.value as E[keyof E]) : null
				);
			} else {
				(onChange as DropdownArraySingleProps<T>['onChange'])(
					item ? item.value : null,
					item?.object ?? null
				);
			}
		},
		[isEnum, onChange]
	);

	const emitMultiChange = useCallback(
		(nextValues: string[]) => {
			if (isEnum) {
				(onChange as DropdownEnumMultiProps<E>['onChange'])(
					nextValues.length ? (nextValues as E[keyof E][]) : null
				);
			} else {
				const objects = nextValues
					.map((id) => itemsMap[id]?.object)
					.filter((o): o is T => !!o);
				(onChange as DropdownArrayMultiProps<T>['onChange'])(
					nextValues.length ? nextValues : null,
					objects
				);
			}
		},
		[isEnum, itemsMap, onChange]
	);

	const handleSelect = useCallback(
		(item: DropdownItem<T>) => {
			if (getOptionDisabled?.(item)) {
				return;
			}

			setKnownLabels((previous) => {
				if (previous.get(item.value) === item.label) {
					return previous;
				}
				const next = new Map(previous);
				next.set(item.value, item.label);
				return next;
			});

			if (multiple) {
				const next = isSelected(item.value)
					? selectedValues.filter((v) => v !== item.value)
					: [...selectedValues, item.value];
				emitMultiChange(next);
				return;
			}

			emitSingleChange(item);
			setQuery('');
			handleClose();
		},
		[
			emitMultiChange,
			emitSingleChange,
			getOptionDisabled,
			handleClose,
			isSelected,
			multiple,
			selectedValues,
		]
	);

	const handleClear = useCallback(() => {
		if (multiple) {
			emitMultiChange([]);
		} else {
			emitSingleChange(null);
		}
		resetSearch();
	}, [emitMultiChange, emitSingleChange, multiple, resetSearch]);

	const handleQueryChange = useCallback(
		(text: string) => {
			setQuery(text);
			if (!isOpen && isTriggerCombobox) {
				setIsOpen(true);
			}
			if (onSearch) {
				runSearch(text);
			}
			if (isTriggerCombobox && !multiple && text === '') {
				emitSingleChange(null);
			}
		},
		[
			emitSingleChange,
			isOpen,
			isTriggerCombobox,
			multiple,
			onSearch,
			runSearch,
		]
	);

	const handlePanelSearchClear = useCallback(() => {
		setQuery('');
		cancelSearch();
		onSearch?.(null);
	}, [cancelSearch, onSearch]);

	const showClear =
		!hiddenClearIcon &&
		!isDisabled &&
		(multiple ? selectedValues.length > 0 : !!value);

	const handlePanelScroll = useCallback(
		(event: NativeSyntheticEvent<NativeScrollEvent>) => {
			if (!onScrollEnd || loading || !hasNext) {
				return;
			}
			const { layoutMeasurement, contentOffset, contentSize } = event.nativeEvent;
			const isEnd =
				contentSize.height - contentOffset.y <= layoutMeasurement.height + 50;
			if (isEnd) {
				onScrollEnd();
			}
		},
		[hasNext, loading, onScrollEnd]
	);

	const listProps = {
		items: filteredItems,
		multiple,
		isSelected,
		onSelect: handleSelect,
		getOptionDisabled,
		onScrollEnd,
		hasNext,
		loading,
	};

	const triggerValue = isTriggerCombobox
		? (isOpen ? query : displayText || query)
		: displayText;

	const panelSearchInput = isPanelSearch ? (
		<View
			style={{
				borderBottomWidth: 1,
				borderBottomColor: '#e5e7eb',
				paddingHorizontal: 8,
				paddingVertical: 8,
			}}
		>
			<TmobisSimpleInput
				placeholder={searchPlaceholder}
				value={query}
				onChangeText={handleQueryChange}
				leftIconName="SearchIcon"
				showClearIcon={query.length > 0}
				onRemove={handlePanelSearchClear}
				isLoading={loading}
			/>
		</View>
	) : null;

	const triggerInput = (
		<TmobisSimpleInput
			placeholder={placeholder}
			value={triggerValue}
			onChangeText={isTriggerCombobox ? handleQueryChange : undefined}
			onFocus={isTriggerCombobox ? handleOpen : undefined}
			isReadonly={!isTriggerCombobox}
			isDisabled={isDisabled}
			isInvalid={isInvalid}
			isLoading={loading && !isPanelSearch}
			showClearIcon={showClear}
			onRemove={handleClear}
			rightIconName="ChevronDownIcon"
			onRightIconClick={handleOpen}
			pointerEvents={isTriggerCombobox ? undefined : 'none'}
		/>
	);

	const trigger = isTriggerCombobox ? (
		triggerInput
	) : (
		<Pressable onPress={handleOpen} disabled={isDisabled}>
			{triggerInput}
		</Pressable>
	);

	const triggerWithAnchor = (
		<View ref={anchorRef} collapsable={false} style={{ width: '100%' }}>
			{trigger}
		</View>
	);

	const useSheet = Platform.OS !== 'web';

	if (useSheet) {
		return (
			<>
				{triggerWithAnchor}
				<Actionsheet isOpen={isOpen} onClose={handleClose}>
					<ActionsheetBackdrop />
					<ActionsheetContent>
						{isPanelSearch ? (
							<>
								<ActionsheetDragIndicatorWrapper>
									<ActionsheetDragIndicator />
								</ActionsheetDragIndicatorWrapper>
								{panelSearchInput}
								<DropdownList
									{...listProps}
									variant="sheet"
									hideSheetChrome
								/>
							</>
						) : (
							<DropdownList {...listProps} variant="sheet" />
						)}
					</ActionsheetContent>
				</Actionsheet>
			</>
		);
	}

	const panelContent = (
		<>
			{panelSearchInput}
			<DropdownPanelScroll
				onScroll={onScrollEnd ? handlePanelScroll : undefined}
				scrollEventThrottle={16}
				keyboardShouldPersistTaps="handled"
				nestedScrollEnabled
			>
				<DropdownList {...listProps} variant="panel" />
			</DropdownPanelScroll>
			{loading ? (
				<DropdownLoading size="panel">
					<Spinner size="small" />
				</DropdownLoading>
			) : null}
		</>
	);

	return (
		<>
			{triggerWithAnchor}
			<DropdownOverlay isOpen={isOpen} onClose={handleClose}>
				<DropdownBackdrop
					onPress={handleClose}
					accessibilityLabel="Закрыть список"
				/>
				{anchor ? (
					<DropdownPanel placement="portal" anchor={anchor}>
						{panelContent}
					</DropdownPanel>
				) : null}
			</DropdownOverlay>
		</>
	);
}

export { TmobisSimpleDropdown };

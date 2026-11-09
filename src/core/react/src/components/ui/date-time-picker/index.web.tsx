'use client';

import {
	DateTimePickerProvider,
	useDateTimePicker,
} from '@gluestack-ui/core/date-time-picker/creator';
import { PrimitiveIcon } from '@gluestack-ui/core/icon/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
	Pressable,
	Text,
	TextInput,
	View,
	type LayoutRectangle,
} from 'react-native';
import { useGluestackColors } from '../../../gluestack-ui-provider/useGluestackColors';
import {
	Calendar,
	CalendarBody,
	CalendarFooter,
	CalendarGrid,
	CalendarHeader,
	CalendarHeaderMonthSelect,
	CalendarHeaderNextButton,
	CalendarHeaderPrevButton,
	CalendarHeaderYearSelect,
	CalendarWeekDaysHeader,
} from '../calendar';
import { calendarFooterActionStyle } from '../calendar/styles';
import { Portal } from '../portal';
import {
	dateTimePickerIconStyle,
	dateTimePickerInputStyle,
	dateTimePickerPopupOverlayStyle,
	dateTimePickerPopupStyle,
	dateTimePickerStyle,
	dateTimePickerTimeInputStyle,
	dateTimePickerTriggerStyle,
} from './styles';

export type DateTimePickerMode = 'date' | 'time' | 'datetime';

export interface DateTimePickerProps {
	value?: Date;
	onChange?: (date: Date | undefined) => void;
	mode?: DateTimePickerMode;
	minimumDate?: Date;
	maximumDate?: Date;
	locale?: string;
	timeZoneOffsetInMinutes?: number;
	is24Hour?: boolean;
	disabled?: boolean;
	placeholder?: string;
	format?: string;
	display?: 'modal' | 'inline';
	className?: string;
	style?: any;
	children?: React.ReactNode;
}

type IDateTimePickerProps = VariantProps<typeof dateTimePickerStyle> &
	DateTimePickerProps;

const DateTimePicker = React.forwardRef<
	React.ComponentRef<typeof View>,
	IDateTimePickerProps
>(function DateTimePicker(
	{
		className,
		value,
		onChange,
		mode = 'datetime',
		minimumDate,
		maximumDate,
		locale = 'ru-RU',
		timeZoneOffsetInMinutes,
		is24Hour,
		disabled,
		placeholder,
		format,
		display: _display,
		children,
		style,
		...props
	},
	ref
) {
	const anchorRef = useRef<View>(null);

	return (
		<DateTimePickerProvider
			value={value}
			onChange={onChange}
			mode={mode}
			minimumDate={minimumDate}
			maximumDate={maximumDate}
			locale={locale}
			timeZoneOffsetInMinutes={timeZoneOffsetInMinutes}
			is24Hour={is24Hour}
			disabled={disabled}
			placeholder={placeholder}
			format={format}
		>
			<View
				ref={ref}
				className={dateTimePickerStyle({ class: `relative w-full ${className || ''}` })}
				style={style}
				{...props}
			>
				<View ref={anchorRef} collapsable={false} className="w-full">
					{children ?? (
						<DateTimePickerTrigger disabled={disabled}>
							<DateTimePickerInput placeholder={placeholder} />
						</DateTimePickerTrigger>
					)}
				</View>
				<WebDateTimePickerPopup
					anchorRef={anchorRef}
					value={value}
					onChange={onChange}
					mode={mode}
					minimumDate={minimumDate}
					maximumDate={maximumDate}
					locale={locale}
				/>
			</View>
		</DateTimePickerProvider>
	);
});

const POPUP_GAP = 4;
const CALENDAR_WIDTH = 280;

function WebDateTimePickerPopup({
	anchorRef,
	value,
	onChange,
	mode,
	minimumDate,
	maximumDate,
	locale,
}: {
	anchorRef: React.RefObject<View | null>;
	value?: Date;
	onChange?: (date: Date | undefined) => void;
	mode: DateTimePickerMode;
	minimumDate?: Date;
	maximumDate?: Date;
	locale: string;
}) {
	const { isOpen, setIsOpen } = useDateTimePicker();
	const colors = useGluestackColors();
	const [anchor, setAnchor] = useState<LayoutRectangle | null>(null);

	const measureAnchor = useCallback(() => {
		anchorRef.current?.measureInWindow((x, y, width, height) => {
			setAnchor({ x, y, width, height });
		});
	}, [anchorRef]);

	useEffect(() => {
		if (!isOpen) {
			setAnchor(null);
			return;
		}

		const frame = requestAnimationFrame(() => measureAnchor());
		if (typeof window === 'undefined') {
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
	}, [isOpen, measureAnchor]);

	const handleDateSelect = useCallback(
		(date: Date) => {
			const next = new Date(date);
			if (mode === 'datetime' && value) {
				next.setHours(value.getHours(), value.getMinutes(), 0, 0);
			} else if (mode === 'date') {
				next.setHours(0, 0, 0, 0);
			}
			onChange?.(next);
			if (mode === 'date') {
				setIsOpen(false);
			}
		},
		[mode, onChange, setIsOpen, value]
	);

	const handleToday = useCallback(() => {
		const today = new Date();
		if (mode === 'datetime' && value) {
			today.setHours(value.getHours(), value.getMinutes(), 0, 0);
		} else if (mode === 'date') {
			today.setHours(0, 0, 0, 0);
		}
		onChange?.(today);
		if (mode === 'date') {
			setIsOpen(false);
		}
	}, [mode, onChange, setIsOpen, value]);

	const handleTimeChange = useCallback(
		(event: React.ChangeEvent<HTMLInputElement>) => {
			const raw = event.target.value;
			if (!raw) {
				return;
			}
			const [hours, minutes] = raw.split(':').map(Number);
			const next = value ? new Date(value) : new Date();
			next.setHours(hours, minutes, 0, 0);
			onChange?.(next);
		},
		[onChange, value]
	);

	if (!isOpen) {
		return null;
	}

	const showCalendar = mode === 'date' || mode === 'datetime';
	const showTime = mode === 'time' || mode === 'datetime';
	const calendarValue = isValidDate(value) ? value : undefined;
	const calendarMinDate = isValidDate(minimumDate) ? minimumDate : undefined;
	const calendarMaxDate = isValidDate(maximumDate) ? maximumDate : undefined;
	const timeInputValue = calendarValue ? formatTimeForInput(calendarValue) : '';
	const panelBackground = colors.popover || colors.background || '#ffffff';

	return (
		<Portal
			isOpen={isOpen}
			onRequestClose={() => setIsOpen(false)}
			isKeyboardDismissable
			style={{
				position: 'absolute',
				top: 0,
				left: 0,
				right: 0,
				bottom: 0,
				zIndex: 1000,
			}}
		>
			<Pressable
				className={dateTimePickerPopupOverlayStyle({})}
				onPress={() => setIsOpen(false)}
				accessibilityLabel="Закрыть календарь"
			/>
			{anchor ? (
				<View
					className={dateTimePickerPopupStyle({})}
					style={{
						position: 'absolute',
						top: anchor.y + anchor.height + POPUP_GAP,
						left: anchor.x,
						zIndex: 1001,
						width: CALENDAR_WIDTH,
						backgroundColor: panelBackground,
						borderRadius: 8,
						overflow: 'hidden',
						borderWidth: 1,
						borderColor: colors.border || '#e8e8e8',
						shadowColor: '#000',
						shadowOffset: { width: 0, height: 8 },
						shadowOpacity: 0.08,
						shadowRadius: 24,
						elevation: 8,
					}}
				>
					{showCalendar && (
						<Calendar
							mode="single"
							value={calendarValue}
							onValueChange={handleDateSelect}
							minDate={calendarMinDate}
							maxDate={calendarMaxDate}
							locale={locale}
							firstDayOfWeek={1}
							enableMonthYearPicker
							showOutsideDays
							fixedWeeks
							initialMonth={calendarValue}
							className="border-0 shadow-none w-full"
							style={{ backgroundColor: panelBackground }}
						>
							<CalendarHeader>
								<CalendarHeaderPrevButton />
								<View className="flex-1 flex-row items-center justify-center">
									<CalendarHeaderMonthSelect />
									<CalendarHeaderYearSelect />
								</View>
								<CalendarHeaderNextButton />
							</CalendarHeader>
							<CalendarWeekDaysHeader />
							<CalendarBody>
								<CalendarGrid />
							</CalendarBody>
							<CalendarFooter className="w-full items-stretch">
								{showTime && (
									<input
										type="time"
										value={timeInputValue}
										onChange={handleTimeChange}
										className={dateTimePickerTimeInputStyle({ class: 'mt-0 mb-2' })}
										aria-label="Время"
									/>
								)}
								<Pressable
									onPress={handleToday}
									accessibilityLabel="Сегодня"
									className="items-center"
								>
									<Text className={calendarFooterActionStyle({})}>Сегодня</Text>
								</Pressable>
							</CalendarFooter>
						</Calendar>
					)}
					{showTime && !showCalendar && (
						<View
							className="rounded-lg p-3 w-full"
							style={{ backgroundColor: panelBackground }}
						>
							<input
								type="time"
								value={timeInputValue}
								onChange={handleTimeChange}
								className={dateTimePickerTimeInputStyle({ class: 'mt-0' })}
								aria-label="Время"
							/>
						</View>
					)}
				</View>
			) : null}
		</Portal>
	);
}

type IDateTimePickerTriggerProps = VariantProps<
	typeof dateTimePickerTriggerStyle
> &
	React.ComponentProps<typeof Pressable> & {
		className?: string;
		size?: 'sm' | 'md' | 'lg';
		variant?: 'outline' | 'underlined' | 'rounded';
	};

const DateTimePickerTrigger = React.forwardRef<
	React.ComponentRef<typeof Pressable>,
	IDateTimePickerTriggerProps
>(function DateTimePickerTrigger(
	{ className, size = 'md', variant = 'outline', ...props },
	ref
) {
	const { disabled, setIsOpen } = useDateTimePicker();

	return (
		<Pressable
			ref={ref}
			className={dateTimePickerTriggerStyle({
				class: className,
				size,
				variant,
			})}
			disabled={disabled}
			onPress={() => !disabled && setIsOpen(true)}
			{...props}
		/>
	);
});

type IDateTimePickerInputProps = VariantProps<typeof dateTimePickerInputStyle> &
	React.ComponentProps<typeof TextInput> & { className?: string };

const DateTimePickerInput = React.forwardRef<
	React.ComponentRef<typeof TextInput>,
	IDateTimePickerInputProps
>(function DateTimePickerInput({ className, ...props }, ref) {
	const { value, placeholder, format } = useDateTimePicker();

	const displayValue = useMemo(() => {
		if (!value || Number.isNaN(value.getTime())) return '';
		if (format) {
			return formatDate(value, format);
		}
		return value.toLocaleString();
	}, [value, format]);

	return (
		<TextInput
			ref={ref}
			className={dateTimePickerInputStyle({ class: className })}
			value={displayValue}
			placeholder={placeholder}
			editable={false}
			style={{ pointerEvents: 'none' }}
			{...props}
		/>
	);
});

type IDateTimePickerIconProps = VariantProps<typeof dateTimePickerIconStyle> &
	React.ComponentProps<typeof PrimitiveIcon> & { className?: string };

const DateTimePickerIcon = React.forwardRef<
	React.ComponentRef<typeof PrimitiveIcon>,
	IDateTimePickerIconProps
>(function DateTimePickerIcon({ className, size, ...props }, ref) {
	if (typeof size === 'number') {
		return (
			<PrimitiveIcon
				ref={ref}
				{...props}
				className={dateTimePickerIconStyle({ class: className })}
				size={size}
			/>
		);
	}

	return (
		<PrimitiveIcon
			className={dateTimePickerIconStyle({
				class: className,
				size,
			})}
			ref={ref}
			{...props}
		/>
	);
});

function isValidDate(date?: Date): date is Date {
	return date instanceof Date && !Number.isNaN(date.getTime());
}

function formatDate(date: Date, format: string): string {
	const pad = (n: number) => n.toString().padStart(2, '0');
	return format
		.replace('YYYY', date.getFullYear().toString())
		.replace('MM', pad(date.getMonth() + 1))
		.replace('DD', pad(date.getDate()))
		.replace('HH', pad(date.getHours()))
		.replace('mm', pad(date.getMinutes()))
		.replace('ss', pad(date.getSeconds()));
}

function formatTimeForInput(date: Date): string {
	const hours = date.getHours().toString().padStart(2, '0');
	const minutes = date.getMinutes().toString().padStart(2, '0');
	return `${hours}:${minutes}`;
}

export {
	DateTimePicker,
	DateTimePickerIcon,
	DateTimePickerInput,
	DateTimePickerTrigger,
};

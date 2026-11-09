'use client';

import {
  createCalendar,
  type ICalendarProps
} from '@gluestack-ui/core/calendar/creator';
import React from 'react';
import { Pressable, Text, View } from 'react-native';
import {
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  Icon,
} from '../icon';
import { Menu, MenuItem, MenuItemLabel } from '../menu';
import { useGluestackColors } from '../../../gluestack-ui-provider/useGluestackColors';
import {
  calendarBodyStyle,
  calendarDayIndicatorStyle,
  calendarDayStyle,
  calendarDayTextStyle,
  calendarFooterStyle,
  calendarGridStyle,
  calendarHeaderButtonStyle,
  calendarHeaderSelectItemStyle,
  calendarHeaderSelectMenuStyle,
  calendarHeaderSelectStyle,
  calendarHeaderSelectTriggerStyle,
  calendarHeaderStyle,
  calendarHeaderTitleStyle,
  calendarStyle,
  calendarWeekDaysHeaderStyle,
  calendarWeekDayStyle,
  calendarWeekDayTextStyle,
  calendarWeekNumberStyle,
  calendarWeekNumberTextStyle,
  calendarWeekStyle,
} from './styles';

const EN_MONTH_TO_RU: Record<string, string> = {
  January: 'Январь',
  February: 'Февраль',
  March: 'Март',
  April: 'Апрель',
  May: 'Май',
  June: 'Июнь',
  July: 'Июль',
  August: 'Август',
  September: 'Сентябрь',
  October: 'Октябрь',
  November: 'Ноябрь',
  December: 'Декабрь',
};

const RU_WEEKDAYS_MONDAY_FIRST = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'] as const;

function localizeMonthLabel(label: string): string {
  return EN_MONTH_TO_RU[label] ?? label;
}

// Styled Root Component
const CalendarRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  ICalendarProps & React.ComponentProps<typeof View> & { className?: string }
>(({ className, style, ...props }, ref) => {
  const colors = useGluestackColors();
  return (
    <View
      ref={ref}
      {...props}
      className={calendarStyle({ class: className })}
      style={[
        {
          backgroundColor: colors.popover || colors.background || '#ffffff',
          overflow: 'hidden',
        },
        style,
      ]}
    />
  );
});

// Styled Header
const CalendarHeaderRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarHeaderStyle({ class: className })}
      {...props}
    />
  );
});

const CalendarHeaderPrevButtonRoot = React.forwardRef<
  React.ElementRef<typeof Pressable>,
  React.ComponentProps<typeof Pressable> & {
    className?: string;
    disabled?: boolean;
  }
>(({ className, disabled, children, ...props }, ref) => {
  return (
    <Pressable
      ref={ref}
      className={calendarHeaderButtonStyle({ class: className })}
      data-disabled={disabled}
      {...props}
    >
      {children ?? (
        <Icon as={ChevronLeftIcon} size="sm" className="text-muted-foreground" />
      )}
    </Pressable>
  );
});

const CalendarHeaderNextButtonRoot = React.forwardRef<
  React.ElementRef<typeof Pressable>,
  React.ComponentProps<typeof Pressable> & {
    className?: string;
    disabled?: boolean;
  }
>(({ className, disabled, children, ...props }, ref) => {
  return (
    <Pressable
      ref={ref}
      className={calendarHeaderButtonStyle({ class: className })}
      data-disabled={disabled}
      {...props}
    >
      {children ?? (
        <Icon as={ChevronRightIcon} size="sm" className="text-muted-foreground" />
      )}
    </Pressable>
  );
});

const CalendarHeaderTitleRoot = React.forwardRef<
  React.ElementRef<typeof Text>,
  React.ComponentProps<typeof Text> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <Text
      ref={ref}
      className={calendarHeaderTitleStyle({ class: className })}
      {...props}
    />
  );
});

type SelectRootProps = React.ComponentProps<typeof View> & {
  className?: string;
  items?: Array<{ label: string; value: number }>;
  selectedValue?: number;
  onValueChange?: (value: number) => void;
};

const CalendarHeaderSelect = React.forwardRef<
  React.ElementRef<typeof View>,
  SelectRootProps & { fallbackLabel: string }
>(function CalendarHeaderSelect(
  { className, items = [], selectedValue, onValueChange, fallbackLabel, ...props },
  ref
) {
  const selectedItem = items.find((item) => item.value === selectedValue);
  const label = selectedItem?.label ?? fallbackLabel;

  return (
    <View ref={ref} className={calendarHeaderSelectStyle({ class: className })} {...props}>
      <Menu
        placement="bottom"
        offset={4}
        className={calendarHeaderSelectMenuStyle({})}
        trigger={({ ...triggerProps }) => (
          <Pressable
            {...triggerProps}
            className={calendarHeaderSelectTriggerStyle({})}
          >
            <Text className="text-sm font-semibold text-foreground">
              {localizeMonthLabel(label)}
            </Text>
            <Icon as={ChevronDownIcon} size="xs" className="text-muted-foreground" />
          </Pressable>
        )}
      >
        {items.map((item) => (
          <MenuItem
            key={item.value}
            textValue={localizeMonthLabel(item.label)}
            onPress={() => onValueChange?.(item.value)}
            className={calendarHeaderSelectItemStyle({})}
          >
            <MenuItemLabel
              className={
                item.value === selectedValue
                  ? 'text-info font-semibold'
                  : 'text-foreground'
              }
            >
              {localizeMonthLabel(item.label)}
            </MenuItemLabel>
          </MenuItem>
        ))}
      </Menu>
    </View>
  );
});

const CalendarHeaderMonthSelectRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  SelectRootProps
>(({ ...props }, ref) => {
  return <CalendarHeaderSelect ref={ref} fallbackLabel="Месяц" {...props} />;
});

const CalendarHeaderYearSelectRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  SelectRootProps
>(({ ...props }, ref) => {
  return <CalendarHeaderSelect ref={ref} fallbackLabel="Год" {...props} />;
});

// Styled Week Days Header
const CalendarWeekDaysHeaderRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarWeekDaysHeaderStyle({ class: className })}
      {...props}
    />
  );
});

const CalendarWeekDayRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, children, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarWeekDayStyle({ class: className })}
      {...props}
    >
      {typeof children === 'string' ? (
        <Text className={calendarWeekDayTextStyle({ class: '' })}>
          {children}
        </Text>
      ) : (
        children
      )}
    </View>
  );
});

// Styled Body & Grid
const CalendarBodyRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarBodyStyle({ class: className })}
      {...props}
    />
  );
});

const CalendarGridRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarGridStyle({ class: className })}
      {...props}
    />
  );
});

const CalendarWeekRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarWeekStyle({ class: className })}
      {...props}
    />
  );
});

// Styled Day
const CalendarDayRoot = React.forwardRef<
  React.ElementRef<typeof Pressable>,
  React.ComponentProps<typeof Pressable> & {
    'className'?: string;
    'data-state'?: string;
  }
>(({ className, 'data-state': dataState, ...props }, ref) => {
  return (
    <Pressable
      ref={ref}
      className={calendarDayStyle({
        state: dataState as any,
        class: className,
      })}
      {...props}
    />
  );
});

const CalendarDayTextRoot = React.forwardRef<
  React.ElementRef<typeof Text>,
  React.ComponentProps<typeof Text> & { className?: string; state?: any }
>(({ className, state, ...props }, ref) => {
  return (
    <Text
      ref={ref}
      className={calendarDayTextStyle({
        state:
          state?.isSelected && state?.isRangeStart
            ? 'range-start'
            : state?.isSelected && state?.isRangeEnd
              ? 'range-end'
              : state?.isInRange
                ? 'range-middle'
                : state?.isSelected
                  ? 'selected'
                  : state?.isToday
                    ? 'today'
                    : state?.isDisabled
                      ? 'disabled'
                      : state?.isOutsideMonth
                        ? 'outside-month'
                        : 'default',
        class: className,
      })}
      {...props}
    />
  );
});

const CalendarDayIndicatorRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & {
    'className'?: string;
    'data-type'?: string;
  }
>(({ className, 'data-type': dataType, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarDayIndicatorStyle({
        type: dataType as any,
        class: className,
      })}
      {...props}
    />
  );
});

// Styled Week Number
const CalendarWeekNumberRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, children, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarWeekNumberStyle({ class: className })}
      {...props}
    >
      {typeof children === 'string' || typeof children === 'number' ? (
        <Text className={calendarWeekNumberTextStyle({ class: '' })}>
          {children}
        </Text>
      ) : (
        children
      )}
    </View>
  );
});

// Styled Footer
const CalendarFooterRoot = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof View> & { className?: string }
>(({ className, ...props }, ref) => {
  return (
    <View
      ref={ref}
      className={calendarFooterStyle({ class: className })}
      {...props}
    />
  );
});

// Create Calendar using the factory
const UICalendar = createCalendar({
  Root: CalendarRoot,
  Header: CalendarHeaderRoot,
  HeaderPrevButton: CalendarHeaderPrevButtonRoot,
  HeaderNextButton: CalendarHeaderNextButtonRoot,
  HeaderTitle: CalendarHeaderTitleRoot,
  HeaderMonthSelect: CalendarHeaderMonthSelectRoot,
  HeaderYearSelect: CalendarHeaderYearSelectRoot,
  WeekDaysHeader: CalendarWeekDaysHeaderRoot,
  WeekDay: CalendarWeekDayRoot,
  Body: CalendarBodyRoot,
  Grid: CalendarGridRoot,
  Week: CalendarWeekRoot,
  Day: CalendarDayRoot,
  DayText: CalendarDayTextRoot,
  DayIndicator: CalendarDayIndicatorRoot,
  WeekNumber: CalendarWeekNumberRoot,
  Footer: CalendarFooterRoot,
});

// Mode-specific discriminated union props so onValueChange is correctly
// narrowed per mode (prevents TypeScript errors when passing setState).
type OmittedCalendarKeys = 'mode' | 'value' | 'defaultValue' | 'onValueChange';

type SingleModeProps = {
  mode?: 'single';
  value?: Date;
  defaultValue?: Date;
  onValueChange?: (value: Date) => void;
};

type MultipleModeProps = {
  mode: 'multiple';
  value?: Date[];
  defaultValue?: Date[];
  onValueChange?: (value: Date[]) => void;
};

type RangeModeProps = {
  mode: 'range';
  value?: { from: Date; to?: Date };
  defaultValue?: { from: Date; to?: Date };
  onValueChange?: (value: { from: Date; to?: Date }) => void;
};

type CalendarProps = (SingleModeProps | MultipleModeProps | RangeModeProps) &
  Omit<ICalendarProps, OmittedCalendarKeys> &
  Omit<React.ComponentProps<typeof View>, OmittedCalendarKeys> & {
    className?: string;
  };

const CalendarComponent = React.forwardRef<
  React.ElementRef<typeof View>,
  CalendarProps
>((props, ref) => {
  return <UICalendar ref={ref} {...(props as any)} />;
});
CalendarComponent.displayName = 'Calendar';

// Export components
export const Calendar = CalendarComponent;
export const CalendarHeader = UICalendar.Header;
export const CalendarHeaderPrevButton = UICalendar.HeaderPrevButton;
export const CalendarHeaderNextButton = UICalendar.HeaderNextButton;
export const CalendarHeaderTitle = UICalendar.HeaderTitle;
export const CalendarHeaderMonthSelect = UICalendar.HeaderMonthSelect;
export const CalendarHeaderYearSelect = UICalendar.HeaderYearSelect;
export const CalendarWeekDaysHeader = React.forwardRef<
  React.ElementRef<typeof View>,
  React.ComponentProps<typeof UICalendar.WeekDaysHeader>
>(function CalendarWeekDaysHeader({ children, ...props }, ref) {
  return (
    <UICalendar.WeekDaysHeader ref={ref} {...props}>
      {children ??
        RU_WEEKDAYS_MONDAY_FIRST.map((day) => (
          <UICalendar.WeekDay key={day}>{day}</UICalendar.WeekDay>
        ))}
    </UICalendar.WeekDaysHeader>
  );
});
export const CalendarWeekDay = UICalendar.WeekDay;
export const CalendarBody = UICalendar.Body;
export const CalendarGrid = UICalendar.Grid;
export const CalendarWeek = UICalendar.Week;
export const CalendarDay = UICalendar.Day;
export const CalendarDayText = UICalendar.DayText;
export const CalendarDayIndicator = UICalendar.DayIndicator;
export const CalendarWeekNumber = UICalendar.WeekNumber;
export const CalendarFooter = UICalendar.Footer;

// Re-export types
export type {
  CalendarMarker,
  CalendarMarkers, CalendarMode, DayState, ICalendarProps
} from '@gluestack-ui/core/calendar/creator';

export type { CalendarProps };

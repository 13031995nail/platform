import { tva } from '@gluestack-ui/utils/nativewind-utils';

// Main Calendar container
// Fixed width on web so the popup grid does not collapse
export const calendarStyle = tva({
  base: 'w-full web:w-[280px] bg-popover border border-border rounded-lg p-3 gap-1 shadow-soft-4',
  variants: {
    size: {
      sm: 'p-2 gap-1',
      md: 'p-3 gap-1',
      lg: 'p-6 gap-6',
    },
  },
  defaultVariants: {
    size: 'md',
  },
});

// Header
export const calendarHeaderStyle = tva({
  base: 'flex-row items-center justify-between mb-1',
  variants: {},
});

export const calendarHeaderButtonStyle = tva({
  base: 'h-8 w-8 flex items-center justify-center rounded-full hover:bg-muted active:bg-muted/80 data-[disabled=true]:opacity-40 data-[disabled=true]:cursor-not-allowed',
  variants: {},
});

export const calendarHeaderTitleStyle = tva({
  base: 'text-foreground text-sm font-semibold',
  variants: {},
});

// Week Days Header
export const calendarWeekDaysHeaderStyle = tva({
  base: 'flex-row justify-between mb-1',
  variants: {},
});

export const calendarWeekDayStyle = tva({
  base: 'h-8 w-9 shrink-0 items-center justify-center',
  variants: {},
});

export const calendarWeekDayTextStyle = tva({
  base: 'text-muted-foreground text-xs font-medium',
  variants: {},
  parentVariants: {},
});

// Body & Grid
export const calendarBodyStyle = tva({
  base: 'gap-0',
  variants: {},
});

export const calendarGridStyle = tva({
  base: 'gap-0',
  variants: {},
});

export const calendarWeekStyle = tva({
  base: 'flex-row justify-between',
  variants: {},
});

// Day
export const calendarDayStyle = tva({
  base: 'h-9 w-9 shrink-0 items-center justify-center rounded-full relative transition-colors',
  variants: {
    state: {
      'default': 'hover:bg-muted active:bg-muted/80',
      'selected': 'bg-info hover:bg-info active:bg-info',
      'today': 'bg-transparent border border-muted-foreground hover:bg-muted',
      'disabled': 'opacity-40 cursor-not-allowed',
      'outside-month': 'hover:bg-muted/60',
      'range-start': 'bg-info rounded-r-full',
      'range-end': 'bg-info rounded-l-full',
      'range-middle': 'bg-info/20 rounded-none',
    },
  },
});

export const calendarDayTextStyle = tva({
  base: 'text-sm font-normal z-10',
  variants: {
    state: {
      'default': 'text-foreground',
      'selected': 'text-white',
      'today': 'text-foreground',
      'disabled': 'text-muted-foreground',
      'outside-month': 'text-muted-foreground/50',
      'range-start': 'text-white font-semibold',
      'range-end': 'text-white font-semibold',
      'range-middle': 'text-foreground',
    },
  },
});

// Day Indicator (markers/dots)
export const calendarDayIndicatorStyle = tva({
  base: 'absolute bottom-1 flex-row gap-0.5 z-0',
  variants: {
    type: {
      'dot': 'flex-row gap-0.5',
      'multi-dot': 'flex-row gap-0.5',
      'period': 'absolute inset-0 rounded-full opacity-20',
    },
  },
});

export const calendarDotStyle = tva({
  base: 'w-1 h-1 rounded-full',
  variants: {},
});

// Week Number
export const calendarWeekNumberStyle = tva({
  base: 'w-8 items-center justify-center mr-1',
  variants: {},
});

export const calendarWeekNumberTextStyle = tva({
  base: 'text-muted-foreground text-xs font-normal',
  variants: {},
  parentVariants: {},
});

// Footer
export const calendarFooterStyle = tva({
  base: 'mt-2 pt-1 items-center justify-center',
  variants: {},
});

export const calendarFooterActionStyle = tva({
  base: 'text-info text-sm font-medium',
  variants: {},
});

// Month/Year Select
export const calendarHeaderSelectStyle = tva({
  base: 'bg-transparent',
  variants: {},
});

export const calendarHeaderSelectTriggerStyle = tva({
  base: 'px-1 py-1 rounded-md flex-row items-center gap-0.5 hover:bg-muted',
  variants: {},
});

export const calendarHeaderSelectMenuStyle = tva({
  base: 'min-w-[9rem] p-1 rounded-lg bg-popover border border-border shadow-hard-5 max-h-[240px] z-[70]',
  variants: {},
});

export const calendarHeaderSelectItemStyle = tva({
  base: 'min-w-[8.5rem] py-1.5 px-2 rounded',
  variants: {},
});

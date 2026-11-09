'use client';
import { PrimitiveIcon, UIIcon } from '@gluestack-ui/core/icon/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva, useStyleContext, withStyleContext } from '@gluestack-ui/utils/nativewind-utils';
import { styled } from 'nativewind';
import React from 'react';
import { Pressable } from 'react-native';
import { Svg } from 'react-native-svg';
import { Text } from '../text';

const SCOPE = 'CHIP';

const Root = withStyleContext(Pressable, SCOPE);

const chipStyle = tva({
	base: 'flex-row items-center justify-center gap-1 rounded-md px-2.5 py-1 min-w-8 overflow-hidden transition-[color,box-shadow] data-[focus=true]:outline-none data-[focus=true]:web:ring-[3px] data-[focus=true]:web:ring-ring/30 data-[disabled=true]:pointer-events-none data-[disabled=true]:cursor-auto data-[disabled=true]:opacity-50',
	variants: {
		variant: {
			default:
				'bg-secondary data-[hover=true]:bg-secondary/80 data-[active=true]:bg-secondary/70',
			selected:
				'bg-primary data-[hover=true]:bg-primary/90 data-[active=true]:bg-primary/90',
			disabled:
				'bg-muted opacity-50',
			pressable:
				'border border-border bg-background data-[hover=true]:bg-accent data-[active=true]:bg-accent',
		},
	},
});

const chipTextStyle = tva({
	base: 'text-xs font-medium font-sans web:select-none',
	parentVariants: {
		variant: {
			default: 'text-secondary-foreground',
			selected: 'text-primary-foreground',
			disabled: 'text-muted-foreground',
			pressable: 'text-foreground data-[hover=true]:text-accent-foreground data-[active=true]:text-accent-foreground',
		},
	},
});

const chipIconStyle = tva({
	base: 'fill-none h-3.5 w-3.5 pointer-events-none shrink-0',
	parentVariants: {
		variant: {
			default: 'text-secondary-foreground',
			selected: 'text-primary-foreground',
			disabled: 'text-muted-foreground',
			pressable: 'text-foreground data-[hover=true]:text-accent-foreground data-[active=true]:text-accent-foreground',
		},
	},
});

const StyledUIIcon = styled(UIIcon, {
	className: {
		target: 'style',
		nativeStyleToProp: {
			height: true,
			width: true,
			fill: true,
			color: 'classNameColor',
			stroke: true,
		},
	},
});

type IChipProps =
	& Omit<React.ComponentPropsWithoutRef<typeof Root>, 'context'>
	& VariantProps<typeof chipStyle>
	& {
		variant?: 'default' | 'selected' | 'disabled' | 'pressable';
		isDisabled?: boolean;
		className?: string;
	};

const Chip = React.forwardRef<
	React.ComponentRef<typeof Root>,
	IChipProps
>(function Chip({
	children,
	variant = 'default',
	className,
	disabled,
	isDisabled = false,
	...props
}, ref) {
	const chipDisabled = disabled || isDisabled || variant === 'disabled';
	const resolvedVariant = chipDisabled && variant !== 'disabled'
		? 'disabled'
		: variant;

	return (
		<Root
			ref={ref}
			disabled={chipDisabled}
			className={chipStyle({
				variant: resolvedVariant,
				class: className,
			})}
			context={{
				variant: resolvedVariant,
			}}
			{...props}
		>
			{children}
		</Root>
	);
});

type IChipTextProps =
	& React.ComponentPropsWithoutRef<typeof Text>
	& VariantProps<typeof chipTextStyle>;

const ChipText = React.forwardRef<
	React.ComponentRef<typeof Text>,
	IChipTextProps
>(function ChipText({ children, className, size = 'xs', ...props }, ref) {
	const { variant: parentVariant } = useStyleContext(SCOPE);
	return (
		<Text
			ref={ref}
			size={size}
			className={chipTextStyle({
				parentVariants: {
					variant: parentVariant,
				},
				class: className,
			})}
			{...props}
		>
			{children}
		</Text>
	);
});

type IChipIconProps =
	& React.ComponentPropsWithoutRef<typeof PrimitiveIcon>
	& VariantProps<typeof chipIconStyle>
	& {
		size?: number;
	};

const ChipIcon = React.forwardRef<
	React.ComponentRef<typeof Svg>,
	IChipIconProps
>(function ChipIcon({ className, size, ...props }, ref) {
	const { variant: parentVariant } = useStyleContext(SCOPE);

	if (typeof size === 'number') {
		return (
			<StyledUIIcon
				ref={ref}
				{...props}
				className={chipIconStyle({ class: className })}
				size={size}
			/>
		);
	} else if (
		(props?.height !== undefined || props?.width !== undefined) &&
		size === undefined
	) {
		return (
			<StyledUIIcon
				ref={ref}
				{...props}
				className={chipIconStyle({ class: className })}
			/>
		);
	}
	return (
		<StyledUIIcon
			className={chipIconStyle({
				parentVariants: {
					variant: parentVariant,
				},
				class: className,
			})}
			{...props}
			ref={ref}
		/>
	);
});

Chip.displayName = 'Chip';
ChipText.displayName = 'ChipText';
ChipIcon.displayName = 'ChipIcon';

export { Chip, ChipIcon, ChipText, type IChipProps };

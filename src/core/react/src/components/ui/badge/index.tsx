'use client';
import { PrimitiveIcon, UIIcon } from '@gluestack-ui/core/icon/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva, useStyleContext, withStyleContext } from '@gluestack-ui/utils/nativewind-utils';
import { styled } from 'nativewind';
import React from 'react';
import { Svg } from 'react-native-svg';
import { Box } from '../box';
import { Text } from '../text';

const SCOPE = 'BADGE';

const badgeStyle = tva({
	base: 'flex-row items-center justify-center gap-1 rounded-md px-2.5 py-1 min-w-8',
	variants: {
		action: {
			primary: 'bg-primary/15',
			success: 'bg-success/15',
			warning: 'bg-warning/15',
			error: 'bg-error/15',
			info: 'bg-info/15',
			neutral: 'bg-secondary',
		},
	},
});

const badgeTextStyle = tva({
	base: 'text-xs font-medium font-sans',
	parentVariants: {
		action: {
			primary: 'text-primary',
			success: 'text-success',
			warning: 'text-warning',
			error: 'text-error',
			info: 'text-info',
			neutral: 'text-secondary-foreground',
		},
	},
});

const badgeIconStyle = tva({
	base: 'fill-none h-3.5 w-3.5 pointer-events-none shrink-0',
	parentVariants: {
		action: {
			primary: 'text-primary',
			success: 'text-success',
			warning: 'text-warning',
			error: 'text-error',
			info: 'text-info',
			neutral: 'text-secondary-foreground',
		},
	},
});

const ContextBox = withStyleContext(Box, SCOPE);

type IBadgeProps =
	& React.ComponentPropsWithoutRef<typeof ContextBox>
	& VariantProps<typeof badgeStyle>
	& {
		action?: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'neutral';
		className?: string;
	};

const Badge = React.forwardRef<
	React.ComponentRef<typeof ContextBox>,
	IBadgeProps
>(function Badge({
	children,
	action = 'neutral',
	className,
	...props
}, ref) {
	return (
		<ContextBox
			ref={ref}
			className={badgeStyle({ action, class: className })}
			{...props}
			context={{ action }}
		>
			{children}
		</ContextBox>
	);
});

type IBadgeTextProps =
	& React.ComponentPropsWithoutRef<typeof Text>
	& VariantProps<typeof badgeTextStyle>;

const BadgeText = React.forwardRef<
	React.ComponentRef<typeof Text>,
	IBadgeTextProps
>(function BadgeText({ children, className, size = 'xs', ...props }, ref) {
	const { action: parentAction } = useStyleContext(SCOPE);
	return (
		<Text
			ref={ref}
			size={size}
			className={badgeTextStyle({
				parentVariants: {
					action: parentAction,
				},
				class: className,
			})}
			{...props}
		>
			{children}
		</Text>
	);
});

type IBadgeIconProps =
	& React.ComponentPropsWithoutRef<typeof PrimitiveIcon>
	& VariantProps<typeof badgeIconStyle>
	& {
		size?: number;
	};

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

const BadgeIcon = React.forwardRef<
	React.ComponentRef<typeof Svg>,
	IBadgeIconProps
>(function BadgeIcon({ className, size, ...props }, ref) {
	const { action: parentAction } = useStyleContext(SCOPE);

	if (typeof size === 'number') {
		return (
			<StyledUIIcon
				ref={ref}
				{...props}
				className={badgeIconStyle({ class: className })}
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
				className={badgeIconStyle({ class: className })}
			/>
		);
	}
	return (
		<StyledUIIcon
			className={badgeIconStyle({
				parentVariants: {
					action: parentAction,
				},
				class: className,
			})}
			{...props}
			ref={ref}
		/>
	);
});

Badge.displayName = 'Badge';
BadgeText.displayName = 'BadgeText';
BadgeIcon.displayName = 'BadgeIcon';

export { Badge, BadgeIcon, BadgeText, type IBadgeProps };

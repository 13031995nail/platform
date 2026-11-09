'use client';
import { UIIcon } from '@gluestack-ui/core/icon/creator';
import { createRadio } from '@gluestack-ui/core/radio/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva, useStyleContext, withStyleContext } from '@gluestack-ui/utils/nativewind-utils';
import { styled } from 'nativewind';
import React from 'react';
import { Platform, Pressable, Text, View } from 'react-native';

const SCOPE = 'Radio';

const StyledIcon = styled(UIIcon, {
	className: {
		target: 'style',
	},
});

const UIRadio = createRadio({
	Root: (Platform.OS === 'web'
		? withStyleContext(View, SCOPE)
		: withStyleContext(Pressable, SCOPE)) as ReturnType<
			typeof withStyleContext<typeof Pressable>
		>,
	Group: View,
	Icon: StyledIcon,
	Indicator: View,
	Label: Text,
});


const radioStyle = tva({
	base: 'group/radio flex-row items-center justify-start gap-2 web:cursor-pointer data-[disabled=true]:cursor-auto data-[disabled=true]:opacity-50',
});

const radioGroupStyle = tva({
	base: 'gap-3',
});

const radioIconStyle = tva({
	base: 'rounded-full absolute stroke-none fill-primary-foreground h-3 w-3',
});

const radioIndicatorStyle = tva({
	base: 'relative aspect-square rounded-full w-4.5 h-4.5 justify-center items-center shrink-0 bg-input/50 border border-input/25 web:outline-none web:data-[focus-visible=true]:border-ring web:data-[focus-visible=true]:ring-[3px] web:data-[focus-visible=true]:ring-ring/30 data-[checked=true]:bg-primary data-[checked=true]:border-primary data-[invalid=true]:data-[checked=true]:bg-destructive data-[invalid=true]:ring-destructive/30 data-[invalid=true]:border-destructive data-[disabled=true]:cursor-auto',
});

const radioLabelStyle = tva({
	base: 'text-foreground text-sm md:text-base font-body web:select-none web:cursor-pointer data-[disabled=true]:cursor-auto group-data-[disabled=true]/form-control:opacity-50',
});

type IRadioProps = Omit<React.ComponentProps<typeof UIRadio>, 'context'> &
	VariantProps<typeof radioStyle>;
const Radio = React.forwardRef<React.ComponentRef<typeof UIRadio>, IRadioProps>(
	function Radio({ className, size = 'md', ...props }, ref) {
		return (
			<UIRadio
				className={radioStyle({ class: className, size })}
				{...props}
				ref={ref}
				context={{ size }}
			/>
		);
	}
);

type IRadioGroupProps = React.ComponentProps<typeof UIRadio.Group> &
	VariantProps<typeof radioGroupStyle>;
const RadioGroup = React.forwardRef<
	React.ComponentRef<typeof UIRadio.Group>,
	IRadioGroupProps
>(function RadioGroup({ className, ...props }, ref) {
	return (
		<UIRadio.Group
			className={radioGroupStyle({ class: className })}
			{...props}
			ref={ref}
		/>
	);
});

type IRadioIndicatorProps = React.ComponentProps<typeof UIRadio.Indicator> &
	VariantProps<typeof radioIndicatorStyle>;
const RadioIndicator = React.forwardRef<
	React.ComponentRef<typeof UIRadio.Indicator>,
	IRadioIndicatorProps
>(function RadioIndicator({ className, ...props }, ref) {
	const { size } = useStyleContext(SCOPE);
	return (
		<UIRadio.Indicator
			className={radioIndicatorStyle({
				parentVariants: { size },
				class: className,
			})}
			ref={ref}
			{...props}
		/>
	);
});

type IRadioLabelProps = React.ComponentProps<typeof UIRadio.Label> &
	VariantProps<typeof radioIndicatorStyle>;
const RadioLabel = React.forwardRef<
	React.ComponentRef<typeof UIRadio.Label>,
	IRadioLabelProps
>(function RadioLabel({ className, ...props }, ref) {
	const { size } = useStyleContext(SCOPE);
	return (
		<UIRadio.Label
			className={radioLabelStyle({
				parentVariants: { size },
				class: className,
			})}
			ref={ref}
			{...props}
		/>
	);
});

type IRadioIconProps = React.ComponentProps<typeof UIRadio.Icon> &
	VariantProps<typeof radioIconStyle> & {
		height?: number;
		width?: number;
	};
const RadioIcon = React.forwardRef<
	React.ComponentRef<typeof UIRadio.Icon>,
	IRadioIconProps
>(function RadioIcon({ className, size, ...props }, ref) {
	const { size: parentSize } = useStyleContext(SCOPE);

	if (typeof size === 'number') {
		return (
			<UIRadio.Icon
				ref={ref}
				{...props}
				className={radioIconStyle({ class: className })}
				size={size}
			/>
		);
	} else if (
		(props.height !== undefined || props.width !== undefined) &&
		size === undefined
	) {
		return (
			<UIRadio.Icon
				ref={ref}
				{...props}
				className={radioIconStyle({ class: className })}
			/>
		);
	}

	return (
		<UIRadio.Icon
			{...props}
			className={radioIconStyle({
				parentVariants: {
					size: parentSize,
				},
				size,
				class: className,
			})}
			ref={ref}
		/>
	);
});


Radio.displayName = 'Radio';
RadioGroup.displayName = 'RadioGroup';
RadioIndicator.displayName = 'RadioIndicator';
RadioLabel.displayName = 'RadioLabel';
RadioIcon.displayName = 'RadioIcon';

export {
	Radio,
	RadioGroup,
	RadioIcon,
	RadioIndicator,
	RadioLabel,
	type IRadioGroupProps,
	type IRadioProps
};

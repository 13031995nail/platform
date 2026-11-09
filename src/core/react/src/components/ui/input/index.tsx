'use client';
import { UIIcon } from '@gluestack-ui/core/icon/creator';
import { createInput } from '@gluestack-ui/core/input/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva, withStyleContext } from '@gluestack-ui/utils/nativewind-utils';
import { styled } from 'nativewind';
import React from 'react';
import { Pressable, TextInput, View } from 'react-native';

const SCOPE = 'INPUT';

const StyledUIIcon = styled(UIIcon, { className: "style" });

const UIInput = createInput({
	Root: withStyleContext(View, SCOPE),
	Icon: StyledUIIcon,
	Slot: Pressable,
	Input: TextInput,
});


const inputStyle = tva({
	base: 'w-full min-h-10 px-3 flex-row items-center gap-3 bg-input/50 border border-transparent rounded-md overflow-hidden transition-[color,box-shadow] data-[focus=true]:outline-none data-[focus=true]:border-ring data-[focus=true]:web:ring-[3px] data-[focus=true]:web:ring-ring/30 data-[invalid=true]:border-destructive data-[invalid=true]:web:ring-destructive/30 data-[disabled=true]:pointer-events-none data-[disabled=true]:cursor-auto data-[disabled=true]:opacity-50',
});

const inputIconStyle = tva({
	base: 'justify-center items-center text-muted-foreground fill-none',
	variants: {
		size: {
			'2xs': 'h-3 w-3',
			'xs': 'h-3.5 w-3.5',
			'sm': 'h-4 w-4',
			'md': 'h-[18px] w-[18px]',
			'lg': 'h-5 w-5',
			'xl': 'h-6 w-6',
		},
	},
});

const inputSlotStyle = tva({
	base: 'justify-center items-center web:disabled:cursor-auto h-5 w-5',
});

const inputFieldStyle = tva({
	base: 'flex-1 text-foreground text-sm md:text-base h-full placeholder:text-muted-foreground web:outline-none ios:leading-[0px] web:cursor-text web:data-[disabled=true]:cursor-auto',
});

type IInputProps = React.ComponentProps<typeof UIInput> &
	VariantProps<typeof inputStyle> & { className?: string };
const Input = React.forwardRef<React.ComponentRef<typeof UIInput>, IInputProps>(
	function Input({ className, ...props }, ref) {
		return (
			<UIInput
				ref={ref}
				{...props}
				className={inputStyle({ class: className })}
				context={{}}
			/>
		);
	}
);

type IInputIconProps = React.ComponentProps<typeof UIInput.Icon> &
	VariantProps<typeof inputIconStyle> & {
		className?: string;
		height?: number;
		width?: number;
	};

const InputIcon = React.forwardRef<
	React.ComponentRef<typeof UIInput.Icon>,
	IInputIconProps
>(function InputIcon({ className, ...props }, ref) {
	return (
		<UIInput.Icon
			ref={ref}
			{...props}
			className={inputIconStyle({ class: className })}
		/>
	);
});

type IInputSlotProps = React.ComponentProps<typeof UIInput.Slot> &
	VariantProps<typeof inputSlotStyle> & { className?: string };

const InputSlot = React.forwardRef<
	React.ComponentRef<typeof UIInput.Slot>,
	IInputSlotProps
>(function InputSlot({ className, ...props }, ref) {
	return (
		<UIInput.Slot
			ref={ref}
			{...props}
			className={inputSlotStyle({
				class: className,
			})}
		/>
	);
});

type IInputFieldProps = React.ComponentProps<typeof UIInput.Input> &
	VariantProps<typeof inputFieldStyle> & { className?: string };

const InputField = React.forwardRef<
	React.ComponentRef<typeof UIInput.Input>,
	IInputFieldProps
>(function InputField({ className, ...props }, ref) {
	return (
		<UIInput.Input
			ref={ref}
			{...props}
			className={inputFieldStyle({
				class: className,
			})}
		/>
	);
});

Input.displayName = 'Input';
InputIcon.displayName = 'InputIcon';
InputSlot.displayName = 'InputSlot';
InputField.displayName = 'InputField';

export {
	Input,
	InputField,
	InputIcon,
	inputIconStyle,
	InputSlot,
	type IInputFieldProps
};

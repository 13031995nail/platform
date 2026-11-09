'use client';
import { createButton } from '@gluestack-ui/core/button/creator';
import { UIIcon } from '@gluestack-ui/core/icon/creator';
import {
	tva,
	useStyleContext,
	withStyleContext,
	type VariantProps,
} from '@gluestack-ui/utils/nativewind-utils';
import { styled } from 'nativewind';
import React from 'react';
import { ActivityIndicator, Pressable, Text, View } from 'react-native';

const SCOPE = 'BUTTON';

const Root = withStyleContext(Pressable, SCOPE);

const StyledUIIcon = styled(UIIcon, {
	className: "style",
});

const UIButton = createButton({
	Root: Root,
	Text,
	Group: View,
	Spinner: ActivityIndicator,
	Icon: StyledUIIcon,
});

const buttonStyle = tva({
	base: 'h-fit flex-row items-center justify-center gap-2 rounded-md overflow-hidden transition-[color,box-shadow] data-[focus=true]:outline-none data-[focus=true]:border-ring data-[focus=true]:web:ring-[3px] data-[focus=true]:web:ring-ring/30 data-[disabled=true]:pointer-events-none data-[disabled=true]:cursor-auto data-[disabled=true]:opacity-50',
	variants: {
		variant: {
			solid: 'shrink-0 rounded-md',
			link: 'h-auto min-h-0 px-0 py-0 data-[hover=true]:opacity-80 data-[active=true]:opacity-80',
		},
		action: {
			primary: '',
			secondary: '',
			destructive: 'data-[focus=true]:border-destructive data-[focus=true]:web:ring-destructive/30',
		},
		size: {
			md: 'px-4 py-2',
			sm: 'min-h-8 rounded-md px-3 text-xs',
			lg: 'min-h-10 rounded-md px-8',
			icon: 'min-h-9 min-w-9',
		},
	},
	compoundVariants: [{
		variant: 'solid',
		action: 'primary',
		class: 'bg-primary data-[hover=true]:bg-primary/90 data-[active=true]:bg-primary/90',
	}, {
		variant: 'solid',
		action: 'secondary',
		class: 'bg-secondary data-[hover=true]:bg-secondary/90 data-[active=true]:bg-secondary/90',
	}, {
		variant: 'solid',
		action: 'destructive',
		class: 'bg-destructive data-[hover=true]:bg-destructive/90 data-[active=true]:bg-destructive/90',
	}],
});

const buttonTextStyle = tva({
	base: 'web:select-none font-sans font-medium',
	parentVariants: {
		size: {
			md: 'text-base',
			sm: 'text-sm',
			lg: 'text-base',
			icon: 'text-sm',
		},
		variant: {
			solid: '',
			link: '',
		},
		action: {
			primary: '',
			secondary: '',
			destructive: '',
		},
	},
	compoundVariants: [{
		variant: 'solid',
		action: 'primary',
		class: 'text-primary-foreground',
	}, {
		variant: 'solid',
		action: 'secondary',
		class: 'text-secondary-foreground',
	}, {
		variant: 'solid',
		action: 'destructive',
		class: 'text-destructive-foreground',
	}, {
		variant: 'link',
		action: 'primary',
		class: 'text-primary',
	}, {
		variant: 'link',
		action: 'secondary',
		class: 'text-secondary',
	}, {
		variant: 'link',
		action: 'destructive',
		class: 'text-destructive',
	}],
});

const buttonSpinnerStyle = tva({
	base: '',
	parentVariants: {
		size: {
			md: 'h-4 w-4',
			sm: 'h-3 w-3',
			lg: 'h-5 w-5',
			icon: 'h-4 w-4',
		},
	},
});

const buttonIconStyle = tva({
	base: 'fill-none pointer-events-none shrink-0',
	parentVariants: {
		size: {
			md: 'h-4 w-4',
			sm: 'h-3 w-3',
			lg: 'h-5 w-5',
			icon: 'h-4 w-4',
		},
		variant: {
			solid: '',
			link: '',
		},
		action: {
			primary: '',
			secondary: '',
			destructive: '',
		},
	},
	compoundVariants: [{
		variant: 'solid',
		action: 'primary',
		class: 'text-primary-foreground',
	}, {
		variant: 'solid',
		action: 'secondary',
		class: 'text-secondary-foreground',
	}, {
		variant: 'solid',
		action: 'destructive',
		class: 'text-destructive-foreground',
	}, {
		variant: 'link',
		action: 'primary',
		class: 'text-primary',
	}, {
		variant: 'link',
		action: 'secondary',
		class: 'text-secondary',
	}, {
		variant: 'link',
		action: 'destructive',
		class: 'text-destructive',
	}],
});

const buttonGroupStyle = tva({
	base: '',
	variants: {
		space: {
			'xs': 'gap-1',
			'sm': 'gap-2',
			'md': 'gap-3',
			'lg': 'gap-4',
			'xl': 'gap-5',
			'2xl': 'gap-6',
			'3xl': 'gap-7',
			'4xl': 'gap-8',
		},
		isAttached: {
			true: 'gap-0',
		},
		flexDirection: {
			'row': 'flex-row',
			'column': 'flex-col',
			'row-reverse': 'flex-row-reverse',
			'column-reverse': 'flex-col-reverse',
		},
	},
});

type IButtonProps =
	& Omit<React.ComponentPropsWithoutRef<typeof UIButton>, 'context'>
	& VariantProps<typeof buttonStyle>
	& {
		action: 'primary' | 'secondary' | 'destructive'
		className?: string
	};

const Button = React.forwardRef<
	React.ElementRef<typeof UIButton>,
	IButtonProps
>(({
	className,
	variant = 'solid',
	action = 'primary',
	size = 'md',
	...props
}, ref) => {
	return (
		<UIButton
			className={buttonStyle({
				variant,
				action,
				size,
				class: className
			})}
			context={{
				variant,
				action,
				size
			}}
			ref={ref}
			{...props}
		/>
	);
});

type IButtonTextProps =
	& React.ComponentPropsWithoutRef<typeof UIButton.Text>
	& VariantProps<typeof buttonTextStyle>
	& { className?: string };

const ButtonText = React.forwardRef<
	React.ElementRef<typeof UIButton.Text>,
	IButtonTextProps
>(({
	className,
	size,
	...props
}, ref) => {
	const {
		variant: parentVariant,
		action: parentAction,
		size: parentSize
	} = useStyleContext(SCOPE);
	return (
		<UIButton.Text
			className={buttonTextStyle({
				parentVariants: {
					variant: parentVariant,
					action: parentAction,
					size: parentSize
				},
				size,
				class: className,
			})}
			ref={ref}
			{...props}
		/>
	);
});

const ButtonSpinner = React.forwardRef<
	React.ElementRef<typeof UIButton.Spinner>,
	React.ComponentPropsWithoutRef<typeof UIButton.Spinner>
>(({
	className,
	...props
}, ref) => {
	const {
		size: parentSize
	} = useStyleContext(SCOPE);
	return <UIButton.Spinner
		className={buttonSpinnerStyle({
			parentVariants: {
				size: parentSize
			},
			class: className
		})}
		ref={ref}
		{...props}
	/>;
});

type IButtonIcon =
	& React.ComponentPropsWithoutRef<typeof UIButton.Icon>
	& VariantProps<typeof buttonIconStyle>
	& {
		className?: string | undefined;
		as?: React.ElementType;
		height?: number;
		width?: number;
	};

const ButtonIcon = React.forwardRef<
	React.ElementRef<typeof UIButton.Icon>,
	IButtonIcon
>(({
	className,
	size,
	...props
}, ref) => {

	const {
		variant: parentVariant,
		action: parentAction,
		size: parentSize
	} = useStyleContext(SCOPE);
	if (typeof size === 'number') {
		return (
			<UIButton.Icon
				className={buttonIconStyle({
					class: className
				})}
				size={size}
				ref={ref}
				{...props}
			/>
		);
	} else if ((props.height !== undefined || props.width !== undefined) && size === undefined) {
		return (
			<UIButton.Icon
				className={buttonIconStyle({
					class: className
				})}
				ref={ref}
				{...props}
			/>
		);
	}
	return (
		<UIButton.Icon
			className={buttonIconStyle({
				parentVariants: {
					variant: parentVariant,
					action: parentAction,
					size: parentSize
				},
				size,
				class: className,
			})}
			ref={ref}
			{...props}
		/>
	);
});

type IButtonGroupProps =
	& React.ComponentPropsWithoutRef<typeof UIButton.Group>
	& VariantProps<typeof buttonGroupStyle>;

const ButtonGroup = React.forwardRef<
	React.ElementRef<typeof UIButton.Group>,
	IButtonGroupProps
>(({
	className,
	space = 'md',
	isAttached = false,
	flexDirection = 'column',
	...props
}, ref) => {
	return (
		<UIButton.Group
			className={buttonGroupStyle({
				class: className,
				space,
				isAttached,
				flexDirection,
			})}
			ref={ref}
			{...props}
		/>
	);
});

Button.displayName = 'Button';
ButtonText.displayName = 'ButtonText';
ButtonSpinner.displayName = 'ButtonSpinner';
ButtonIcon.displayName = 'ButtonIcon';
ButtonGroup.displayName = 'ButtonGroup';

export {
	Button,
	ButtonGroup,
	ButtonIcon,
	ButtonSpinner,
	ButtonText,
	type IButtonProps
};


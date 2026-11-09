'use client';
import { createSwitch } from '@gluestack-ui/core/switch/creator';
import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva, withStyleContext } from '@gluestack-ui/utils/nativewind-utils';
import React from 'react';
import { Switch as RNSwitch, useColorScheme } from 'react-native';

function useAppTheme() {
	const colorScheme = useColorScheme();
	if (colorScheme === 'dark') {
		return {
			trackColor: {
				inactive: '#353535',
				active: '#f05b25',
			},
			thumbColor: '#ffffff',
			activeThumbColor: '#ffffff',
			ios_backgroundColor: '#353535'
		}
	} else {
		return {
			trackColor: {
				inactive: '#353535',
				active: '#f05b25',
			},
			thumbColor: '#ffffff',
			activeThumbColor: '#ffffff',
			ios_backgroundColor: '#353535'
		}
	}
}

const UISwitch = createSwitch({
	Root: withStyleContext(RNSwitch),
});

const switchStyle = tva({
	base: 'web:cursor-pointer disabled:cursor-auto data-[focus=true]:outline-none data-[focus=true]:border-ring data-[focus=true]:web:ring-[3px] data-[focus=true]:web:ring-ring/30 data-[disabled=true]:opacity-50',

	variants: {
		size: {
			sm: 'scale-[0.75]',
			md: '',
			lg: 'scale-[1.25]',
		},
	},
});

type ISwitchProps = React.ComponentProps<typeof UISwitch> &
	VariantProps<typeof switchStyle>;
const Switch = React.forwardRef<
	React.ComponentRef<typeof UISwitch>,
	ISwitchProps
>(function Switch({ className, size = 'md', ...props }, ref) {
	const activeTheme = useAppTheme();
	return (
		<UISwitch
			ref={ref}
			{...props}
			className={switchStyle({ size, class: className })}
			trackColor={{ false: activeTheme.trackColor.inactive, true: activeTheme.trackColor.active }}
			thumbColor={activeTheme.thumbColor}
			activeThumbColor={activeTheme.activeThumbColor}
			ios_backgroundColor={activeTheme.ios_backgroundColor}
		/>
	);
});

Switch.displayName = 'Switch';
export { Switch, type ISwitchProps };

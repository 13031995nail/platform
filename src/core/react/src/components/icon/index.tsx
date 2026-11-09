import * as GluestackIcons from '@tmobis/core/components/ui/icon';
import { Icon, iconStyle } from '@tmobis/core/components/ui/icon';

type TmobisIconProps = {
	name: string;
	className?: string;
	size?: '2xs' | 'xs' | 'sm' | 'md' | 'lg' | 'xl';
};

const getIconByName = (name: string) => {
	// @ts-ignore
	return GluestackIcons[name];
};


const TmobisIcon = ({ name, className, size = 'md' }: TmobisIconProps) => {
	const IconComponent = typeof name === 'string'
		? getIconByName(name)
		: name;

	if (!IconComponent) {
		return null;
	}

	return <Icon as={IconComponent} size={size} className={className} />;
};


export { getIconByName, TmobisIcon, iconStyle }
import { getIconByName } from '@tmobis/core/components/icon';
import {
	Badge,
	BadgeIcon,
	BadgeText,
	type IBadgeProps,
} from '@tmobis/core/components/ui/badge';

type TmobisBadgeProps =
	& IBadgeProps
	& {
		label?: string;
		action?: 'primary' | 'success' | 'warning' | 'error' | 'info' | 'neutral';
		iconName?: string;
	};

const TmobisBadge = ({
	label,
	action = 'neutral',
	iconName,
	children,
	...props
}: TmobisBadgeProps) => {
	const IconComponent = iconName
		? getIconByName(iconName)
		: null;

	return (
		<Badge
			action={action}
			{...props}
		>
			{IconComponent
				? <BadgeIcon as={IconComponent} />
				: null}
			{label
				? <BadgeText>{label}</BadgeText>
				: children}
		</Badge>
	);
};

export { TmobisBadge, type TmobisBadgeProps };

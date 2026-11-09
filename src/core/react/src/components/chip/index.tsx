import { getIconByName } from '@tmobis/core/components/icon';
import {
	Chip,
	ChipIcon,
	ChipText,
	type IChipProps,
} from '@tmobis/core/components/ui/chip';

type TmobisChipProps =
	& IChipProps
	& {
		label?: string;
		variant?: 'default' | 'selected' | 'disabled' | 'pressable';
		iconName?: string;
	};

const TmobisChip = ({
	label,
	variant = 'default',
	iconName,
	isDisabled = false,
	children,
	...props
}: TmobisChipProps) => {
	const IconComponent = iconName
		? getIconByName(iconName)
		: null;

	return (
		<Chip
			variant={variant}
			isDisabled={isDisabled || variant === 'disabled'}
			accessibilityLabel={label}
			{...props}
		>
			{IconComponent
				? <ChipIcon as={IconComponent} />
				: null}
			{label
				? <ChipText>{label}</ChipText>
				: children}
		</Chip>
	);
};

export { TmobisChip, type TmobisChipProps };

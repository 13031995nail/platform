import { TmobisIcon } from '@tmobis/core/components/icon';
import {
	Button,
	ButtonText,
	IButtonProps,
} from '@tmobis/core/components/ui/button';

type TmobisButtonProps =
	& IButtonProps
	& {
		label?: string;
		variant?: 'solid' | 'link';
		size?: 'sm' | 'md' | 'lg';
		action?: 'primary' | 'secondary' | 'destructive';
		leftIconName?: string;
		rightIconName?: string;
	};

const TmobisButton = ({
	label = 'Button',
	variant = 'solid',
	size = 'md',
	action = 'primary',
	isDisabled = false,
	leftIconName,
	rightIconName,
	...props
}: TmobisButtonProps) => {

	return (
		<Button
			variant={variant}
			action={action}
			size={size}
			isDisabled={isDisabled}
			accessibilityLabel={label}
			{...props}
		>
			{leftIconName
				? <TmobisIcon name={leftIconName} />
				: null}
			<ButtonText>{label}</ButtonText>
			{rightIconName
				? <TmobisIcon name={rightIconName} />
				: null}
		</Button>
	);
};

export { TmobisButton };

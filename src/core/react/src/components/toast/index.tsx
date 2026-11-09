import {
	Toast,
	ToastDescription,
	ToastTitle,
	useToast as useGluestackToast,
} from '@tmobis/core/components/ui/toast';
import { VStack } from '@tmobis/core/components/ui/vstack';
import type { ReactNode } from 'react';

type BasicToastProps = {
	id?: string;
	title: string;
	text?: string;
	action?: 'error' | 'warning' | 'success' | 'info' | 'muted';
	variant?: 'solid' | 'outline';
};

type ToastPlacement =
	| 'top'
	| 'top right'
	| 'top left'
	| 'bottom'
	| 'bottom left'
	| 'bottom right';

type ShowToastParams = {
	title: string;
	text?: string;
	placement?: ToastPlacement;
	render?: (props: BasicToastProps) => ReactNode;
};

const BasicToast = ({
	id,
	title,
	text,
	action = 'muted',
	variant = 'solid',
}: BasicToastProps) => {
	const nativeId = id
		? `toast-${id}`
		: undefined;
	const titleComponent = title
		? <ToastTitle>{title}</ToastTitle>
		: null;
	const textComponent = text
		? <ToastDescription>{text}</ToastDescription>
		: null;

	return (
		<Toast
			nativeID={nativeId}
			action={action}
			variant={variant}
		>
			<VStack space='xs'>
				{titleComponent}
				{textComponent}
			</VStack>
		</Toast>
	);
};

const useTmobisToast = () => {
	const toast = useGluestackToast();

	const show = ({
		title,
		text,
		placement = 'top',
		render,
	}: ShowToastParams) => {
		return toast.show({
			placement,
			render: ({ id }) => {
				const toastParams: BasicToastProps = {
					id,
					title,
					text,
				};
				const toastComponent = render
					? render(toastParams)
					: <BasicToast {...toastParams} />;

				return toastComponent;
			},
		});
	};

	return {
		show,
		close: toast.close,
		closeAll: toast.closeAll,
		isActive: toast.isActive,
	};
};

export {
	BasicToast,
	useTmobisToast,
	type BasicToastProps,
	type ShowToastParams,
	type ToastPlacement,
};

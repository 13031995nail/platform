import { Alert, AlertButton, AlertOptions } from 'react-native';

type TmobisAlertPressHandler = (() => void) | undefined;

type TmobisAlertProps = {
	title: string;
	message?: string;
	onClose?: TmobisAlertPressHandler;
	isDenyVisible?: boolean;
	onDeny?: TmobisAlertPressHandler;
	onDenyTitle?: string;
	isApproveVisible?: boolean;
	onApprove?: TmobisAlertPressHandler;
	onApproveTitle?: string;
	isOpen?: boolean;
};

type TmobisAsyncAlertProps = Omit<TmobisAlertProps, 'onApprove' | 'onDeny' | 'onClose'>;

const TmobisAlert = ({
	title,
	message,
	onClose,
	isDenyVisible = true,
	onDeny,
	onDenyTitle = 'Нет',
	isApproveVisible = true,
	onApprove,
	onApproveTitle = 'Да',
	isOpen = true,
}: TmobisAlertProps): void => {
	if (!isOpen) {
		return;
	}
	const alertButtons: AlertButton[] = [];

	if (isDenyVisible) {
		alertButtons.push({
			text: onDenyTitle,
			onPress: onDeny,
			style: 'cancel',
		});
	}

	if (isApproveVisible) {
		alertButtons.push({
			text: onApproveTitle,
			onPress: onApprove,
		});
	}

	const alertOptions: AlertOptions = onClose
		? {
			cancelable: true,
			onDismiss: onClose,
		}
		: {};

	Alert.alert(title, message, alertButtons, alertOptions);
};


const TmobisAsyncAlert = ({
	...alertProps
}: TmobisAsyncAlertProps): Promise<boolean> => {
	return new Promise((resolve, reject) => {
		TmobisAlert({
			...alertProps,
			onApprove: () => resolve(true),
			onDeny: () => resolve(false),
			onClose: () => reject(),
		});
	});
};

export { TmobisAlert, TmobisAsyncAlert };

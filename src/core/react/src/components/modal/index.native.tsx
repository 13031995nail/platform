import { TmobisButton } from '@tmobis/core/components/button';
import {
	BottomSheetBackdrop,
	BottomSheetModal,
	BottomSheetView,
} from '@gorhom/bottom-sheet';
import { Heading } from '@tmobis/core/components/ui/heading';
import { CloseIcon, Icon } from '@tmobis/core/components/ui/icon';
import { Text } from '@tmobis/core/components/ui/text';
import type { BottomSheetBackdropProps } from '@gorhom/bottom-sheet';
import type { ReactNode } from 'react';
import { useCallback, useEffect, useMemo, useRef } from 'react';
import { ActivityIndicator, GestureResponderEvent, Pressable, StyleSheet, View } from 'react-native';

type TmobisModalPressHandler = (((event: GestureResponderEvent) => void) & ((event: GestureResponderEvent) => void)) | null | undefined;

type TmobisModalSize = 'md' | 'lg' | 'xlg' | 'full' | undefined;

type TmobisModalPromise = {
	resolve: () => void;
	reject: () => void;
};

interface TmobisModalProps {
	isOpen: boolean;
	onClose?: TmobisModalPressHandler;
	isLoading?: boolean;
	size?: TmobisModalSize;
	showCloseButton?: boolean;
	title?: string;
	message?: string;
	children?: ReactNode;
	promise?: TmobisModalPromise;
	isDenyVisible?: boolean;
	onDeny?: TmobisModalPressHandler;
	onDenyTitle?: string;
	isApproveVisible?: boolean;
	onApprove?: TmobisModalPressHandler;
	onApproveTitle?: string;
}

type TmobisAsyncModalProps = TmobisModalProps & {
	promise?: TmobisModalPromise;
};

const TmobisModal = ({
	isOpen,
	onClose,
	isLoading = false,
	size = 'md',
	showCloseButton = true,
	title = 'Сообщение',
	message,
	children,
	isDenyVisible = true,
	onDeny,
	onDenyTitle = 'Отменить',
	isApproveVisible = true,
	onApprove,
	onApproveTitle = 'Подтвердить',
}: TmobisModalProps) => {
	const bottomSheetRef = useRef<BottomSheetModal>(null);

	const handleClose = useCallback(() => {
		onClose?.({} as GestureResponderEvent);
	}, [onClose]);

	const handleBottomSheetClose = useCallback(() => {
		handleClose();
	}, [handleClose]);

	useEffect(() => {
		if (isOpen) {
			bottomSheetRef.current?.present();
		} else {
			bottomSheetRef.current?.close();
		}
	}, [isOpen]);

	const titleComponent =
		<Heading size='lg'>
			{title ? title : null}
		</Heading>;
	const closeButtonComponent =
		<Pressable onPress={handleClose} style={styles.closeButton}>
			<Icon as={CloseIcon} />
		</Pressable>;
	const messageComponent = message
		? <Text>{message}</Text>
		: null;
	const headerComponent = (
		<View style={styles.header}>
			{titleComponent}
			{!isLoading && showCloseButton
				? closeButtonComponent
				: null
			}
		</View>
	);
	const bodyComponent = isLoading
		? <ActivityIndicator />
		: children ?? messageComponent;
	const denyButtonComponent = isDenyVisible
		? <TmobisButton
			size='sm'
			action='secondary'
			title={onDenyTitle}
			onPress={onDeny}
		/>
		: null;
	const approveButtonComponent = isApproveVisible
		? <TmobisButton
			size='sm'
			title={onApproveTitle}
			onPress={onApprove}
		/>
		: null;
	const footerComponent = !isLoading && (denyButtonComponent || approveButtonComponent)
		? <>
			{denyButtonComponent}
			{approveButtonComponent}
		</>
		: null;

	return (
		<BottomSheetModal
			ref={bottomSheetRef}
			enablePanDownToClose={!isLoading}
			onDismiss={handleBottomSheetClose}
		>
			<BottomSheetView style={styles.content}>
				{headerComponent}
				{bodyComponent
					? <View style={styles.body}>{bodyComponent}</View>
					: null
				}
				{footerComponent
					? <View style={styles.footer}>{footerComponent}</View>
					: null
				}
			</BottomSheetView>
		</BottomSheetModal>
	);
};

const TmobisAsyncModal = ({
	promise,
	onClose,
	onDeny,
	onApprove,
	isApproveVisible = true,
	...modalProps
}: TmobisAsyncModalProps) => {
	const handleClose = (event: GestureResponderEvent) => {
		onClose?.(event);
	};
	const handleDeny = (event: GestureResponderEvent) => {
		promise?.reject();
		onDeny?.(event);
	};
	const handleApprove = (event: GestureResponderEvent) => {
		promise?.resolve();
		onApprove?.(event);
	};

	return (
		<TmobisModal
			{...modalProps}
			onClose={handleClose}
			onDeny={handleDeny}
			onApprove={handleApprove}
			isApproveVisible={isApproveVisible}
		/>
	);
};

const styles = StyleSheet.create({
	content: {
		paddingBottom: 24,
		paddingHorizontal: 24,
	},
	header: {
		alignItems: 'center',
		flexDirection: 'row',
		justifyContent: 'space-between',
	},
	closeButton: {
		padding: 4,
	},
	body: {
		marginBottom: 24,
		marginTop: 8,
	},
	footer: {
		alignItems: 'center',
		flexDirection: 'row',
		gap: 8,
		justifyContent: 'flex-end',
	},
});

export { TmobisAsyncModal, TmobisModal, type TmobisModalPromise };

import { TmobisButton } from '@tmobis/core/components/button';
import { Heading } from '@tmobis/core/components/ui/heading';
import { CloseIcon, Icon } from '@tmobis/core/components/ui/icon';
import {
	Modal,
	ModalBackdrop,
	ModalBody,
	ModalCloseButton,
	ModalContent,
	ModalFooter,
	ModalHeader,
} from '@tmobis/core/components/ui/modal';
import { Text } from '@tmobis/core/components/ui/text';
import type { ReactNode } from 'react';
import { ActivityIndicator, GestureResponderEvent } from 'react-native';

type TmobisModalPressHandler = (((event: GestureResponderEvent) => void) & ((event: GestureResponderEvent) => void));

type TmobisModalPromise = {
	resolve: () => void;
	reject: () => void;
};

type TmobisModalProps = {
	isOpen: boolean;
	onClose: TmobisModalPressHandler;
	isLoading?: boolean;
	size?: 'md' | 'lg' | 'xlg' | 'full' | undefined;
	showCloseButton?: boolean;
	title?: string;
	message?: string;
	children?: ReactNode;
	isDenyVisible?: boolean;
	onDeny?: TmobisModalPressHandler;
	onDenyTitle?: string;
	isApproveVisible?: boolean;
	onApprove?: TmobisModalPressHandler;
	onApproveTitle?: string;
};

type TmobisAsyncModalProps = TmobisModalProps & {
	promise?: TmobisModalPromise;
};

const TmobisModal = ({
	isOpen = false,
	onClose,
	isLoading = false,
	size = 'md',
	showCloseButton = true,
	title = 'Сообщение',
	message,
	children,
	isDenyVisible = false,
	onDeny,
	onDenyTitle = 'Отменить',
	isApproveVisible = false,
	onApprove,
	onApproveTitle = 'Подтвердить',
}: TmobisModalProps) => {
	const titleComponent =
		<Heading size='md'>
			{title ? title : null}
		</Heading>;
	const closeButtonComponent =
		<ModalCloseButton>
			<Icon as={CloseIcon} />
		</ModalCloseButton>;
	const headerComponent =
		<ModalHeader>
			{titleComponent}
			{!isLoading && showCloseButton
				? closeButtonComponent
				: null
			}
		</ModalHeader>;
	const messageComponent = message
		? <Text>{message}</Text>
		: null;
	const bodyContent = isLoading
		? <ActivityIndicator />
		: children ?? messageComponent;
	const bodyComponent = bodyContent
		? <ModalBody>
			{bodyContent}
		</ModalBody>
		: null;
	const denyButtonComponent = isDenyVisible
		? <TmobisButton
			size='sm'
			action='secondary'
			label={onDenyTitle}
			onPress={onDeny}
		/>
		: null;
	const approveButtonComponent = isApproveVisible
		? <TmobisButton
			size='sm'
			label={onApproveTitle}
			onPress={onApprove}
		/>
		: null;
	const footerComponent = !isLoading && (denyButtonComponent || approveButtonComponent)
		? <ModalFooter>
			{denyButtonComponent}
			{approveButtonComponent}
		</ModalFooter>
		: null;

	return (
		<Modal
			isOpen={isOpen}
			onClose={onClose}
			size={size}
			closeOnOverlayClick={!isLoading}
		>
			<ModalBackdrop />
			<ModalContent>
				{headerComponent}
				{bodyComponent}
				{footerComponent}
			</ModalContent>
		</Modal>
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
			onClose={handleClose}
			onDeny={handleDeny}
			onApprove={handleApprove}
			isApproveVisible={isApproveVisible}
			{...modalProps}
		/>
	);
};

export {
	TmobisAsyncModal,
	TmobisModal,
	type TmobisModalProps,
	type TmobisModalPromise
};

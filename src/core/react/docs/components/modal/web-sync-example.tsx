import TmobisButton from '@/components/tmobis-ui/button';
import { TmobisModal } from '@/components/tmobis-ui/modal';
import { useState } from 'react';
import { StyleSheet, View } from 'react-native';

export default function Index() {
	const [isOpened, setIsOpened] = useState(false);
	const [isLoading, setIsLoading] = useState(false);
	const openModal = () => {
		setIsLoading(true);
		setIsOpened(true);
		setTimeout(() => {
			setIsLoading(false);
		}, 3000);
	};
	const onModalApprove = () => {
		console.log('Подтверждено');
		setIsOpened(false);
	};
	const onModalDeny = () => {
		console.log('Отменено');
		setIsOpened(false);
	};
	const onModalClose = () => {
		console.log('Закрыто');
		setIsOpened(false);
	};

	return (
		<View style={styles.container}>
			<TmobisButton onPress={openModal} />
			<TmobisModal
				isOpen={isOpened}
				isLoading={isLoading}
				message='Простое модальное окно'
				onClose={onModalClose}
				onApprove={onModalApprove}
				onDeny={onModalDeny}
			/>
		</View>
	);
}

const styles = StyleSheet.create({
	container: {
		flex: 1,
		alignItems: 'center',
		justifyContent: 'center',
	},
});
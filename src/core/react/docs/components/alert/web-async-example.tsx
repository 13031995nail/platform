import { TmobisAlertPromise, TmobisAsyncAlert } from '@/components/tmobis-ui/alert';
import TmobisButton from '@/components/tmobis-ui/button';
import { useRef, useState } from 'react';
import { StyleSheet, View } from 'react-native';

export default function Index() {
	const [isOpened, setIsOpened] = useState(false);

	const promiseRef = useRef<TmobisAlertPromise | undefined>(undefined);

	const openAsyncAlert = () => {
		setIsOpened(true);

		return new Promise<void>(() => {
			promiseRef.current = {
				resolve: () => {
					console.log('Подтверждено');
					setIsOpened(false);
					promiseRef.current = undefined;
				},
				reject: () => {
					console.log('Отменено');
					setIsOpened(false);
					promiseRef.current = undefined;
				},
			};
		});
	};
	const onAlertClose = () => {
		console.log('Закрыто');
		setIsOpened(false);
		promiseRef.current = undefined;
	};

	return (
		<View style={styles.container}>
			<TmobisButton onPress={() => openAsyncAlert()} />
			<TmobisAsyncAlert
				isOpen={isOpened}
				onClose={onAlertClose}
				promise={promiseRef.current}
				message='Асинхронный алерт'
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

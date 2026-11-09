import { TmobisAlert } from '@/components/tmobis-ui/alert';
import TmobisButton from '@/components/tmobis-ui/button';
import { useState } from 'react';
import { StyleSheet, View } from 'react-native';

export default function Index() {
	const [isOpened, setIsOpened] = useState(false);

	const openAlert = () => {
		setIsOpened(true);
	};
	const onAlertApprove = () => {
		console.log('Подтверждено');
		setIsOpened(false);
	};
	const onAlertDeny = () => {
		console.log('Отменено');
		setIsOpened(false);
	};
	const onAlertClose = () => {
		console.log('Закрыто');
		setIsOpened(false);
	};

	return (
		<View style={styles.container}>
			<TmobisButton onPress={openAlert} />
			<TmobisAlert
				isOpen={isOpened}
				message='Простой алерт'
				onClose={onAlertClose}
				onApprove={onAlertApprove}
				onDeny={onAlertDeny}
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

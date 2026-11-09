import TmobisButton from '@/components/tmobis-ui/button';
import { TmobisAsyncModal, TmobisModalPromise } from '@/components/tmobis-ui/modal';
import { useRef, useState } from 'react';
import { StyleSheet, View } from 'react-native';

export default function Index() {
  const [isOpened, setIsOpened] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const promiseRef = useRef<TmobisModalPromise | undefined>(undefined);

  const openAsyncModal = () => {
    setIsLoading(true);
    setIsOpened(true);
    setTimeout(() => {
      setIsLoading(false);
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
    }, 3000);
  };
	const onModalClose = () => {
		console.log('Закрыто');
		setIsOpened(false);
	};

  return (
    <View style={styles.container}>
      <TmobisButton onPress={() => openAsyncModal()} />
      <TmobisAsyncModal
        isOpen={isOpened}
				isLoading={isLoading}
        onClose={onModalClose}
        promise={promiseRef.current}
        message='Асинхронное модальное окно'
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
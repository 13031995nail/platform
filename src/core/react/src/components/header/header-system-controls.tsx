import { useUserAuth } from '@tmobis/core/auth/session-context';
import { TmobisAlert } from '@tmobis/core/components/alert';
import { HStack } from '@tmobis/core/components/ui/hstack';
import {
  Icon,
  LogOutIcon,
  SettingsIcon,
} from '@tmobis/core/components/ui/icon';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { Text } from '@tmobis/core/components/ui/text';
import { useRouter } from 'expo-router';
import { useState } from 'react';

/**
 * Правая часть системного хедера: имя пользователя, настройки, выход.
 */
const HeaderSystemControls = () => {
  const userAuth = useUserAuth();
  const router = useRouter();
  const [isLogoutModalOpen, setIsLogoutModalOpen] = useState(false);

  const handleOpenLogout = () => {
    setIsLogoutModalOpen(true);
  };

  const handleCloseLogout = () => {
    setIsLogoutModalOpen(false);
  };

  const handleApproveLogout = () => {
    setIsLogoutModalOpen(false);
    userAuth?.logout();
  };

  const handleSettings = () => {
    router.navigate('/settings');
  };

  if (!userAuth) {
    return null;
  }

  const userNameComponent = userAuth.userName
    ? <Text
      size='sm'
      isTruncated
      className='mx-2 max-w-[160px] text-foreground'
    >
      {userAuth.userName}
    </Text>
    : null;

  const settingsButtonComponent =
    <Pressable
      accessibilityLabel='Настройки'
      accessibilityRole='button'
      onPress={handleSettings}
      className='p-2'
    >
      <Icon as={SettingsIcon} className='text-foreground' />
    </Pressable>;

  const logoutButtonComponent =
    <Pressable
      accessibilityLabel='Выйти'
      accessibilityRole='button'
      onPress={handleOpenLogout}
      className='p-2'
    >
      <Icon as={LogOutIcon} className='text-foreground' />
    </Pressable>;

  return (
    <>
      <HStack className='items-center mr-2 gap-1'>
        {userNameComponent}
        {settingsButtonComponent}
        {logoutButtonComponent}
      </HStack>

      <TmobisAlert
        isOpen={isLogoutModalOpen}
        onClose={handleCloseLogout}
        title='Выход'
        message='Вы действительно хотите выйти?'
        isDenyVisible
        onDeny={handleCloseLogout}
        onDenyTitle='Нет'
        isApproveVisible
        onApprove={handleApproveLogout}
        onApproveTitle='Да'
      />
    </>
  );
};

export { HeaderSystemControls };

import { useFocusEffect } from '@react-navigation/native';
import { useHeader } from '@tmobis/core/components/header/context';
import { SettingsView } from '@tmobis/core/components/settings';
import { useCallback } from 'react';
import { useTranslation } from 'react-i18next';

/**
 * Экран настроек приложения: грид акцентных действий и список «кнопка + описание».
 */
const SettingsScreen = () => {
  const { setHeaderData } = useHeader();
  const { t } = useTranslation();
  const title = t('settings.caption', { defaultValue: 'Настройки' });

  useFocusEffect(
    useCallback(() => {
      setHeaderData({ title });
    }, [setHeaderData, title])
  );

  return <SettingsView />;
};

export default SettingsScreen;

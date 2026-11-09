import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import Constants from 'expo-constants';
import { useUserAuth } from '@tmobis/core/auth/session-context';
import { TmobisAlert } from '@tmobis/core/components/alert';
import { TmobisButton } from '@tmobis/core/components/button';
import { TmobisSimpleRadio } from '@tmobis/core/components/checkbox';
import { TmobisModal } from '@tmobis/core/components/modal';
import { RadioGroup } from '@tmobis/core/components/ui/radio';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import {
  useAppPreferences,
  type AppThemePreference,
} from './preferences-provider';
import {
  CORE_SETTINGS_GROUP,
  CORE_SETTINGS_GROUP_ORDER,
  type SettingsItem,
} from './types';
import { SettingsItemRow } from '@tmobis/core/components/settings/item-row';

const LOCALE_OPTIONS = [
  { key: 'ru', labelKey: 'settings.locale.ru' },
  { key: 'en', labelKey: 'settings.locale.en' },
] as const;

const THEME_OPTIONS: { key: AppThemePreference; labelKey: string }[] = [
  { key: 'light', labelKey: 'settings.theme.light' },
  { key: 'dark', labelKey: 'settings.theme.dark' },
  { key: 'system', labelKey: 'settings.theme.system' },
];

type ChoiceModalProps = {
  isOpen: boolean;
  title: string;
  approveTitle: string;
  value: string;
  options: { key: string; label: string }[];
  onChange: (value: string) => void;
  onApprove: () => void;
  onClose: () => void;
};

function ChoiceModal({
  isOpen,
  title,
  approveTitle,
  value,
  options,
  onChange,
  onApprove,
  onClose,
}: ChoiceModalProps) {
  return (
    <TmobisModal
      isOpen={isOpen}
      onClose={onClose}
      title={title}
      isApproveVisible
      onApproveTitle={approveTitle}
      onApprove={onApprove}
    >
      <RadioGroup value={value} onChange={onChange}>
        <VStack space="md">
          {options.map((option) => (
            <TmobisSimpleRadio
              key={option.key}
              value={option.key}
              label={option.label}
            />
          ))}
        </VStack>
      </RadioGroup>
    </TmobisModal>
  );
}

/**
 * Строка выбора языка приложения.
 */
export function LocaleSettingsRow({ item }: { item: SettingsItem }) {
  const { t } = useTranslation();
  const { locale, setLocale } = useAppPreferences();
  const [isOpen, setIsOpen] = useState(false);
  const [draft, setDraft] = useState(locale);

  const open = () => {
    setDraft(locale);
    setIsOpen(true);
  };

  const approve = () => {
    void setLocale(draft);
    setIsOpen(false);
  };

  return (
    <>
      <SettingsItemRow
        caption={t('settings.locale.title', { defaultValue: item.caption })}
        description={t(`settings.locale.${locale}`, { defaultValue: locale })}
        onPress={open}
      />
      <ChoiceModal
        isOpen={isOpen}
        title={t('settings.locale.modal-title')}
        approveTitle={t('settings.locale.modal-approve')}
        value={draft}
        options={LOCALE_OPTIONS.map((option) => ({
          key: option.key,
          label: t(option.labelKey),
        }))}
        onChange={setDraft}
        onApprove={approve}
        onClose={() => setIsOpen(false)}
      />
    </>
  );
}

/**
 * Строка выбора темы приложения.
 */
export function ThemeSettingsRow({ item }: { item: SettingsItem }) {
  const { t } = useTranslation();
  const { theme, setTheme } = useAppPreferences();
  const [isOpen, setIsOpen] = useState(false);
  const [draft, setDraft] = useState(theme);

  const open = () => {
    setDraft(theme);
    setIsOpen(true);
  };

  const approve = () => {
    void setTheme(draft as AppThemePreference);
    setIsOpen(false);
  };

  return (
    <>
      <SettingsItemRow
        caption={t('settings.theme.title', { defaultValue: item.caption })}
        description={t(`settings.theme.${theme}`, { defaultValue: theme })}
        onPress={open}
      />
      <ChoiceModal
        isOpen={isOpen}
        title={t('settings.theme.modal-title')}
        approveTitle={t('settings.theme.modal-approve')}
        value={draft}
        options={THEME_OPTIONS.map((option) => ({
          key: option.key,
          label: t(option.labelKey),
        }))}
        onChange={(value) => setDraft(value as AppThemePreference)}
        onApprove={approve}
        onClose={() => setIsOpen(false)}
      />
    </>
  );
}

/**
 * Строка «О приложении».
 */
export function AboutSettingsRow({ item }: { item: SettingsItem }) {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);
  const appName = Constants.expoConfig?.name ?? 'T-Mobis';
  const version = Constants.expoConfig?.version ?? '';

  return (
    <>
      <SettingsItemRow
        caption={t('settings.about.title', { defaultValue: item.caption })}
        description={version || t('settings.about.description')}
        onPress={() => setIsOpen(true)}
      />
      <TmobisModal
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
        title={t('settings.about.modal-title')}
      >
        <VStack space="md">
          <Text>{appName}</Text>
          {version ? (
            <Text>
              {t('settings.about.version')}: {version}
            </Text>
          ) : null}
        </VStack>
      </TmobisModal>
    </>
  );
}

/**
 * Строка выхода из аккаунта.
 */
export function LogoutSettingsRow({ item }: { item: SettingsItem }) {
  const { t } = useTranslation();
  const userAuth = useUserAuth();
  const [isOpen, setIsOpen] = useState(false);

  if (!userAuth) {
    return null;
  }

  const approve = () => {
    setIsOpen(false);
    void userAuth.logout();
  };

  return (
    <>
      <SettingsItemRow
        caption={t('settings.logout.title', { defaultValue: item.caption })}
        description={userAuth.userName || t('settings.logout.description')}
        control={
          <TmobisButton
            className="shrink-0"
            action="secondary"
            label={t('settings.logout.title')}
            onPress={() => setIsOpen(true)}
          />
        }
      />
      <TmobisAlert
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
        title={t('settings.logout.confirmTitle')}
        message={t('settings.logout.confirmMessage')}
        isDenyVisible
        onDeny={() => setIsOpen(false)}
        onDenyTitle={t('settings.logout.no')}
        isApproveVisible
        onApprove={approve}
        onApproveTitle={t('settings.logout.yes')}
      />
    </>
  );
}

/**
 * Дефолтные пункты настроек ядра.
 */
export function getCoreSettingsItems(): SettingsItem[] {
  return [
    {
      id: 'core.locale',
      variant: 'row',
      group: CORE_SETTINGS_GROUP,
      groupOrder: CORE_SETTINGS_GROUP_ORDER,
      order: 10,
      caption: 'Язык',
      component: LocaleSettingsRow,
    },
    {
      id: 'core.theme',
      variant: 'row',
      group: CORE_SETTINGS_GROUP,
      groupOrder: CORE_SETTINGS_GROUP_ORDER,
      order: 20,
      caption: 'Тема',
      component: ThemeSettingsRow,
    },
    {
      id: 'core.about',
      variant: 'row',
      group: CORE_SETTINGS_GROUP,
      groupOrder: CORE_SETTINGS_GROUP_ORDER,
      order: 30,
      caption: 'О приложении',
      component: AboutSettingsRow,
    },
    {
      id: 'core.logout',
      variant: 'row',
      group: CORE_SETTINGS_GROUP,
      groupOrder: CORE_SETTINGS_GROUP_ORDER,
      order: 40,
      caption: 'Выход',
      component: LogoutSettingsRow,
    },
  ];
}

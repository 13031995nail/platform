import type { Resource } from 'i18next';

export const settingsLocales: Resource = {
  ru: {
    translation: {
      settings: {
        caption: 'Настройки',
        group: {
          general: 'Общее',
        },
        locale: {
          title: 'Язык',
          description: 'Язык интерфейса приложения',
          'modal-title': 'Выбор языка приложения',
          'modal-approve': 'Выбрать',
          ru: 'Русский',
          en: 'English',
        },
        theme: {
          title: 'Тема',
          description: 'Оформление приложения',
          'modal-title': 'Выбор темы',
          'modal-approve': 'Выбрать',
          light: 'Светлая',
          dark: 'Тёмная',
          system: 'Системная',
        },
        about: {
          title: 'О приложении',
          description: 'Сведения о версии',
          'modal-title': 'О приложении',
          version: 'Версия',
        },
        logout: {
          title: 'Выход',
          description: 'Выйти из аккаунта',
          confirmTitle: 'Выход',
          confirmMessage: 'Вы действительно хотите выйти?',
          yes: 'Да',
          no: 'Нет',
        },
      },
    },
  },
  en: {
    translation: {
      settings: {
        caption: 'Settings',
        group: {
          general: 'General',
        },
        locale: {
          title: 'Language',
          description: 'Application interface language',
          'modal-title': 'Choose application language',
          'modal-approve': 'Select',
          ru: 'Русский',
          en: 'English',
        },
        theme: {
          title: 'Theme',
          description: 'Application appearance',
          'modal-title': 'Choose theme',
          'modal-approve': 'Select',
          light: 'Light',
          dark: 'Dark',
          system: 'System',
        },
        about: {
          title: 'About',
          description: 'Version information',
          'modal-title': 'About the application',
          version: 'Version',
        },
        logout: {
          title: 'Log out',
          description: 'Sign out of the account',
          confirmTitle: 'Log out',
          confirmMessage: 'Are you sure you want to log out?',
          yes: 'Yes',
          no: 'No',
        },
      },
    },
  },
};

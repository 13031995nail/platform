import ModuleBase from "@tmobis/core/modules/module";
import { default as ru } from '../translations/ru.json';
import ScreenData from "@tmobis/core/routes/screen-data";
import { Ionicons } from '@expo/vector-icons';
import { Resource } from "i18next";
import type {
    DesktopItem,
    DesktopLayoutConfig,
} from '@tmobis/core/desktop/types';
import type { SettingsItem } from '@tmobis/core/settings/types';
import { Alert } from 'react-native';
import { showComponentLab } from '@/constants/feature-flags';

/**
 * React модуль приложения.
 */
export default class AppModule extends ModuleBase {
    locales?: Resource | undefined = {
        ru: {
            translation: ru
        }
    };

    getScreens(): ScreenData[] {
        return [
            {
                route: 'index',
                label: 'Главная',
                group: 'Test',
                // Не primary: лендинг — /desktop (CoreModule)
                icon: ({ size, color }) => (
                    <Ionicons name="home-outline" size={size} color={color} />
                ),
                roles: ['admin']
            },
            {
                route: 'timer',
                label: 'Таймер',
                group: 'Other',
                primary: true,
                priority: 2,
                icon: ({ size, color }) => (
                    <Ionicons name="timer-outline" size={size} color={color} />
                )
            },
            ...(showComponentLab ? [{
                route: 'component-lab',
                label: 'Тестирование компонентов',
                group: 'Разработка',
                roles: ['admin'],
                icon: ({ size, color }: { size: number; color: string }) => (
                    <Ionicons name="flask-outline" size={size} color={color} />
                )
            }] : [])
        ];
    }

    /**
     * Полное переопределение плиток рабочего стола на уровне приложения.
     * Без этого метода CompositionRoot соберёт getDesktopItems() из модулей.
     */
    getDesktopItemsOverride(): DesktopItem[] {
        return [
            {
                id: 'work-shifts',
                caption: 'Смены',
                title: 'Список смен',
                href: '/work-shifts',
                order: 1,
                icon: ({ size, color }) => (
                    <Ionicons name="calendar-outline" size={size} color={color} />
                ),
            },
            {
                id: 'timer',
                caption: 'Таймер',
                title: 'Таймер',
                href: '/timer',
                order: 2,
                icon: ({ size, color }) => (
                    <Ionicons name="timer-outline" size={size} color={color} />
                ),
            },
            {
                id: 'users',
                caption: 'Пользователи',
                title: 'Пользователи',
                href: '/users',
                order: 3,
                roles: ['admin'],
                icon: ({ size, color }) => (
                    <Ionicons name="people-outline" size={size} color={color} />
                ),
            },
            ...(showComponentLab ? [{
                id: 'component-lab',
                caption: 'Компоненты',
                title: 'Тестирование компонентов',
                href: '/component-lab',
                order: 4,
                roles: ['admin'],
                icon: ({ size, color }: { size: number; color: string }) => (
                    <Ionicons name="flask-outline" size={size} color={color} />
                ),
            }] : []),
        ];
    }

    /**
     * Смена колонок и размера иконок рабочего стола.
     */
    getDesktopConfig(): Partial<DesktopLayoutConfig> {
        return {
            columns: 3,
            iconSize: 80,
            gap: 24,
        };
    }

    /**
     * Пример пунктов настроек модуля: акцентная кнопка и строка с описанием.
     */
    getSettingsItems(): SettingsItem[] {
        return [
            {
                id: 'app.check-updates',
                variant: 'action',
                group: 'Настройка приложения',
                groupOrder: 10,
                order: 10,
                caption: 'Проверить обновления',
                onPress: () => {
                    Alert.alert('', 'Последняя версия приложения уже установлена.');
                },
            },
            {
                id: 'app.reload-dicts',
                variant: 'action',
                group: 'Настройка справочников',
                groupOrder: 20,
                order: 10,
                caption: 'Перезагрузка справочников',
                onPress: () => {
                    Alert.alert('', 'Перезагрузка справочников недоступна в шаблоне.');
                },
            },
            {
                id: 'app.clear-lists',
                variant: 'row',
                group: 'Настройка приложения',
                groupOrder: 10,
                order: 20,
                caption: 'Очистить настройки списков',
                description: 'Сброс сохранённых фильтров, сортировок и ширины колонок',
                onPress: () => {
                    Alert.alert('', 'Настройки списков очищены.');
                },
            },
        ];
    }
};

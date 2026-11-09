import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { useColorScheme as useSystemColorScheme } from 'react-native';
import i18n from 'i18next';
import {
  getStoredValue,
  setStoredValue,
} from '@tmobis/core/storage/async-storage';
import {
  SETTINGS_LOCALE_KEY,
  SETTINGS_THEME_KEY,
} from './types';

export type AppThemePreference = 'light' | 'dark' | 'system';

export type AppPreferences = {
  locale: string;
  theme: AppThemePreference;
  resolvedTheme: 'light' | 'dark';
  setLocale: (locale: string) => Promise<void>;
  setTheme: (theme: AppThemePreference) => Promise<void>;
};

const AppPreferencesContext = createContext<AppPreferences | undefined>(undefined);

function resolveTheme(
  theme: AppThemePreference,
  systemScheme: string | null | undefined,
): 'light' | 'dark' {
  if (theme === 'system') {
    return systemScheme === 'dark' ? 'dark' : 'light';
  }
  return theme;
}

/**
 * Провайдер сохранённых языка и темы приложения.
 */
export function AppPreferencesProvider({ children }: { children: ReactNode }) {
  const systemScheme = useSystemColorScheme();
  const [locale, setLocaleState] = useState(() => i18n.language || 'ru');
  const [theme, setThemeState] = useState<AppThemePreference>('system');

  useEffect(() => {
    let cancelled = false;

    void (async () => {
      const [storedLocale, storedTheme] = await Promise.all([
        getStoredValue(SETTINGS_LOCALE_KEY),
        getStoredValue(SETTINGS_THEME_KEY),
      ]);

      if (cancelled) {
        return;
      }

      if (storedLocale) {
        setLocaleState(storedLocale);
        if (i18n.language !== storedLocale) {
          await i18n.changeLanguage(storedLocale);
        }
      }

      if (storedTheme === 'light' || storedTheme === 'dark' || storedTheme === 'system') {
        setThemeState(storedTheme);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, []);

  const setLocale = useCallback(async (nextLocale: string) => {
    setLocaleState(nextLocale);
    await i18n.changeLanguage(nextLocale);
    await setStoredValue(SETTINGS_LOCALE_KEY, nextLocale);
  }, []);

  const setTheme = useCallback(async (nextTheme: AppThemePreference) => {
    setThemeState(nextTheme);
    await setStoredValue(SETTINGS_THEME_KEY, nextTheme);
  }, []);

  const resolvedTheme = resolveTheme(theme, systemScheme);

  const value = useMemo<AppPreferences>(
    () => ({
      locale,
      theme,
      resolvedTheme,
      setLocale,
      setTheme,
    }),
    [locale, theme, resolvedTheme, setLocale, setTheme],
  );

  return (
    <AppPreferencesContext.Provider value={value}>
      {children}
    </AppPreferencesContext.Provider>
  );
}

export function useAppPreferences(): AppPreferences {
  const context = useContext(AppPreferencesContext);
  if (!context) {
    throw new Error('useAppPreferences must be used within AppPreferencesProvider');
  }
  return context;
}

export function useOptionalAppPreferences(): AppPreferences | undefined {
  return useContext(AppPreferencesContext);
}

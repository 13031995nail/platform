import React, {
  createContext, useContext, useEffect, useMemo, useState, PropsWithChildren, useCallback
} from 'react';
import { Platform } from 'react-native';
import type { ApiClient } from '@tmobis/core/api/create-api-client';
import {StoredTokens, AuthState} from '../lib/types';
import tokenStorage from "@tmobis/auth-keycloak/lib/storage";
import authClient from "@tmobis/auth-keycloak/lib/auth-client";
import {SessionContextProvider, type UserAuth} from '@tmobis/core/auth/session-context';
import {keycloakSessionProvider} from '../lib/session-provider';
import type { IRolesManager } from '@tmobis/core/security/roles-manager';
import { RolesManager } from '@tmobis/core/security/roles-manager';
import { setRolesManager } from '@tmobis/core/security/permissions';

interface AuthContextType {
  authState: AuthState;
  login: () => Promise<void>;
  logout: () => Promise<void>;
  rolesManager: IRolesManager;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

/**
 * Обеспечение доступа к контексту аутентификации для дочерних компонентов.
 * Управление состоянием и предоставление данных аутентификации.
 */
export function AuthProvider({ children, client }: PropsWithChildren<{ client: ApiClient }>) {
  const [authState, setAuthStateInternal] = useState<AuthState>({ status: 'authenticating' });

  const checkAuth = useCallback(async () => {
    setAuthStateInternal({ status: 'authenticating' });
    try {
      const storedTokens = await tokenStorage.loadTokens();
      if (storedTokens) {
        setAuthStateInternal({ status: 'authenticated', tokens: storedTokens });
      } else {
        setAuthStateInternal({ status: 'idle' });
      }
    } catch (error) {
      if (error instanceof Error) {
        const isStorageCorrupted = error.message === 'storage_corrupted';
        if (isStorageCorrupted) {
          await tokenStorage.clearTokens();
        }
        setAuthStateInternal({
          status: 'error',
          error: { kind: isStorageCorrupted ? 'storage_corrupted' : 'authorization_failed', message: error.message },
        });
      } else {
        setAuthStateInternal({
          status: 'error',
          error: { kind: 'authorization_failed', message: 'Unknown error during auth check' },
        });
      }
    }
  }, []);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  const onLogin = useCallback(async () => {
    setAuthStateInternal({ status: 'authenticating' });
    try {
      const tokens = await authClient.login(client);
      setAuthStateInternal({ status: 'authenticated', tokens });
    } catch (error: any) {
      if (error instanceof Error && error.message === 'authorization_failed') {
        setAuthStateInternal({ status: 'idle' });
      } else {
        setAuthStateInternal({
          status: 'error',
          error: { kind: 'authorization_failed', message: error instanceof Error ? error.message : 'Authorization failed' },
        });
      }
    }
  }, [client]);

  const onLogout = useCallback(async () => {
    setAuthStateInternal({ status: 'authenticating' });
    try {
      const storedTokens = await tokenStorage.loadTokens();
      const idToken = storedTokens?.idToken;
      if (idToken) {
        await authClient.logout(idToken);
      }
      await tokenStorage.clearTokens();
    } catch (error: any) {
    }

    // Для натива (iOS/Android) сразу переключаем режим.
    // Для веба logout вызывает редирект, состояние останется authenticating.
    if (Platform.OS !== 'web') {
      setAuthStateInternal({ status: 'idle' });
    }
  }, []);
  
  const userAuth = useMemo<UserAuth | null>(() => {
    if (authState.status !== 'authenticated') {
      return null;
    }

    return {
      userName: authState.tokens.backendAccessToken.userName,
      logout: onLogout,
    };
  }, [authState, onLogout]);

  // Создание rolesManager на основе токенов из authState
  const rolesManager = React.useMemo(() => {
    const roles = authState.status === 'authenticated' ? authState.tokens.backendAccessToken?.roles : '';
    const manager = RolesManager.fromString(roles ?? '');
    // Установка глобального менеджера ролей для getCurrentUserRoles
    setRolesManager(manager);
    return manager;
  }, [authState]);

  return (
    <SessionContextProvider
      provider={keycloakSessionProvider}
      userAuth={userAuth}
    >
      <AuthContext.Provider value={{ authState, login: onLogin, logout: onLogout, rolesManager }}>
        {children}
      </AuthContext.Provider>
    </SessionContextProvider>
  );
}

/**
 * Получение данных и методов аутентификации из контекста.
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth должен использоваться внутри AuthProvider.');
  }
  
  return context;
}
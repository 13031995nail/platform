import { BackendAccessToken } from './types';
import { isApiError } from '@tmobis/core/api/api-error';
import type { ApiClient } from '@tmobis/core/api/create-api-client';
import tokenStorage from "@tmobis/auth-keycloak/lib/storage";
import authClient from "@tmobis/auth-keycloak/lib/auth-client";

/**
 * Получение идентификатора сотрудника текущего пользователя из сессии.
 */
export const getCurrentEmployeeId = async (): Promise<string | undefined> => {
  const storedTokens = await tokenStorage.loadTokens();
  const backend = storedTokens?.backendAccessToken;
  return backend?.employee || undefined;
};

/**
 * Выход из системы и Keycloak с очисткой сессии в хранилище.
 */
export const logout = async (): Promise<void> => {
  try {
    const storedTokens = await tokenStorage.loadTokens();
    if (storedTokens?.idToken) {
      await authClient.logout(storedTokens.idToken);
    }
    await tokenStorage.clearTokens();
  } catch {
  }
};

/**
 * Получение токена приложения от бэкенда с помощью токена Keycloak.
 * Использует заголовок Authorization для передачи токена Keycloak.
 * Бэкенд возвращает объект с токеном и данными пользователя.
 */
export const getBackendToken = async (keycloakAccessToken: string, client: ApiClient): Promise<BackendAccessToken> => {
  try {
    const response = await client.post<BackendAccessToken>(
      '/api/auth/keycloak',
      {},
      {
        headers: {
          'Content-Type': 'application/json',
          'accessToken': keycloakAccessToken,
        },
      }
    );
    return response.data;
  } catch (error: unknown) {
    if (isApiError(error)) {
      throw error;
    }
    const message = error instanceof Error ? error.message : String(error);
    throw new Error(`Failed to get backend token: ${message}`);
  }
};
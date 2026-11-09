/**
 * Конфигурация Keycloak.
 */
export interface KeycloakConfig {
  issuer: string;
  clientId: string;
  nativeRedirectUrl: string;
  webRedirectUrl: string;
  scopes: string[];
}

/**
 * Токен приложения от бэкенда.
 */
export interface BackendAccessToken {
  accessToken: string;
  employee?: string;
  roles?: string;
  town?: string;
  user?: string;
  userName?: string;
}

/**
 * Сохраненные токены.
 */
export interface StoredTokens {
  accessToken: string;
  idToken: string;
  backendAccessToken: BackendAccessToken;
}

/**
 * Интерфейс хранилища токенов.
 */
export interface TokenStorage {
  saveTokens(tokens: StoredTokens): Promise<void>;
  loadTokens(): Promise<StoredTokens | null>;
  clearTokens(): Promise<void>;
}

/**
 * Ошибки аутентификации.
 */
export type AuthError =
  | { kind: 'authorization_failed'; message: string }
  | { kind: 'storage_corrupted'; message: string }
  | { kind: 'logout_failed'; message: string };

/**
 * Состояние аутентификации.
 */
export type AuthState =
  | { status: 'idle' }
  | { status: 'authenticating' }
  | { status: 'authenticated'; tokens: StoredTokens; }
  | { status: 'error'; error: AuthError };
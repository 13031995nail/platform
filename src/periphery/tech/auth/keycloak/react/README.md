# @tmobis/auth-keycloak

Модуль аутентификации через Keycloak для React Native и Web приложений.

## Архитектура

Пакет использует платформозависимую логику аутентификации в одном файле:

- **Native (iOS/Android):** `WebBrowser.openAuthSessionAsync` из `expo-web-browser` с `expo-crypto` для PKCE
- **Web:** `WebBrowser.openAuthSessionAsync` из `expo-web-browser` с `window.crypto.subtle` для PKCE

Все остальные части (AuthState, AuthProvider, useAuth, storage) общие и платформонезависимые.

### Модули

- **storage** - модуль хранения токенов с платформозависимыми реализациями:
  - `storage.native.ts` - использует `expo-secure-store` для безопасного хранения в OS keychain
  - `storage.web.ts` - использует `localStorage` для браузерного хранения

- **auth-client** - модуль аутентификации (единая реализация):
  - `auth-client/index.ts` - содержит всю логику OAuth flow с платформозависимыми функциями:
    - `generateCodeChallenge()` - использует `window.crypto.subtle` для веба и `expo-crypto` для натива
    - `logout()` - для веба делает редирект на Keycloak, для натива пустой

## Установка

```bash
yarn add @tmobis/auth-keycloak
```

## Настройка

### Переменные окружения

| Переменная | Описание | Пример |
|-----------|----------|--------|
| `EXPO_PUBLIC_KEYCLOAK_ISSUER` | URL сервера Keycloak | `https://kk.timepath.ru/realms/2234` |
| `EXPO_PUBLIC_KEYCLOAK_CLIENT_ID` | ID клиента в Keycloak | `api` |
| `EXPO_PUBLIC_KEYCLOAK_REDIRECT_URL` | Redirect URL для натива | `expoapp://oauthredirect` |
| `EXPO_PUBLIC_KEYCLOAK_WEB_REDIRECT_URL` | Redirect URL для веба | `https://your-web-domain/redirect` |

### Настройка Keycloak

1. Создайте клиента в Keycloak с типом `Public`
2. Включите Standard Flow, отключите Implicit Flow
3. Настройте PKCE с методом `S256`
4. Добавьте redirect URIs:
   - Для натива: `your-app-scheme://oauthredirect`
   - Для веба: `https://your-web-domain/redirect`
5. Установите Web Origins для веб-клиента

## Использование

### Оборачивание приложения в AuthProvider

```tsx
import { AuthProvider } from '@tmobis/auth-keycloak';

function App() {
  return (
    <AuthProvider>
      <RootNavigator />
    </AuthProvider>
  );
}
```

### Использование хуков

```tsx
import { useAuth } from '@tmobis/auth-keycloak';

function ProtectedScreen() {
  const { authState, login, logout } = useAuth();

  switch (authState.status) {
    case 'authenticated':
      return <MainContent user={authState.user} />;
    case 'idle':
      return <Button onPress={login}>Войти</Button>;
    case 'error':
      return <ErrorMessage error={authState.error.message} />;
  }
}
```

## Состояния аутентификации

- `idle` - нет сохраненных токенов, ожидание входа
- `authenticating` - процесс входа в систему
- `authenticated` - успешная аутентификация с валидными токенами
- `error` - ошибка аутентификации (содержит `AuthError`)

## Методы

### login()
Инициирует OAuth flow через Keycloak. Для веба открывает модальное окно, для натива - системный браузер.

### logout()
Выход из системы. Для веба делает редирект на Keycloak end session endpoint, для натива только очищает локальные токены.

## Импорты по модулям

```ts
// Типы
import type { AuthState, AuthError, StoredTokens, DecodedIdToken } from '@tmobis/auth-keycloak/types';

// Основной экспорт
import { AuthProvider, useAuth, keycloakConfig } from '@tmobis/auth-keycloak';

// Строковый экспорт по пути
import { authClient } from '@tmobis/auth-keycloak/lib/auth-client';
import tokenStorage from '@tmobis/auth-keycloak/lib/storage';
import { getBackendToken } from '@tmobis/auth-keycloak/lib/auth-service';
```

## Безопасность

- **Native:** Токены хранятся в OS keychain через `expo-secure-store`
- **Web:** Токены хранятся в `localStorage` (согласованная уязвимость к XSS)

## Типы

```ts
interface AuthState {
  status: 'idle' | 'authenticating' | 'authenticated' | 'error';
  tokens?: StoredTokens;
  user?: DecodedIdToken;
  error?: AuthError;
}

interface AuthError {
  kind: 'authorization_failed' | 'storage_corrupted' | 'logout_failed';
  message: string;
}

interface StoredTokens {
  accessToken: string;
  idToken: string;
  accessTokenExpirationDate: string;
  backendAccessToken?: BackendAccessToken;
}

interface BackendAccessToken {
  accessToken: string;
  employee?: string;
  roles?: string[];
  town?: string;
  user?: string;
  userName?: string;
}

interface DecodedIdToken {
  sub: string;
  email?: string;
  preferred_username?: string;
  exp: number;
  [key: string]: unknown;
}
```

## Примечания

- Используется PKCE (Proof Key for Code Exchange) flow для безопасной аутентификации
- Access token не обновляется автоматически - при истечении срока действия требуется повторный вход
- Для получения `backendAccessToken` вызывается `getBackendToken()` из `auth-service.ts`
- Для веба `logout()` вызывает редирект на Keycloak end session endpoint, что приведет к возврату в приложение
- Для натива `logout()` просто очищает локальные токены без обращения к Keycloak
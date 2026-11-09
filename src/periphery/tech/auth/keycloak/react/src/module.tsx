import ModuleBase from '@tmobis/core/modules/module';
import type ScreenData from '@tmobis/core/routes/screen-data';
import type { RequireContext } from 'expo-router';
import type { ReactNode } from 'react';
import { ProtectedRoute } from './components/protected-route';
import { maybeCompleteAuthSession } from './lib/web-browser';
import { configureApiAuth, type ApiClient } from '@tmobis/core/api/create-api-client';
import { bumpApiClientQueryScope } from '@tmobis/core/api/query/query-scope';
import type { ModuleLifecycleContext } from '@tmobis/core/modules/module';
import { resolveSessionIdentity } from './lib/session-identity';
import tokenStorage, { subscribeToTokens } from './lib/storage';

export default class KeycloakModule extends ModuleBase {
  private unsubscribeTokens?: () => void;
  private sessionIdentity: string | null = null;
  private sessionInitialized = false;

  initStatic() {
    maybeCompleteAuthSession();
  }

  configureApiClient(client: ApiClient) {
    configureApiAuth(client, async () => {
      const tokens = await tokenStorage.loadTokens();
      return tokens?.backendAccessToken?.accessToken;
    });
  }

  async initialize({ apiClient }: ModuleLifecycleContext) {
    this.unsubscribeTokens?.();
    this.sessionInitialized = false;
    this.unsubscribeTokens = subscribeToTokens(tokens => {
      const nextIdentity = resolveSessionIdentity(tokens);
      // Перевыпуск токенов того же пользователя не сбрасывает запросы.
      if (!this.sessionInitialized || this.sessionIdentity !== nextIdentity) {
        this.sessionIdentity = nextIdentity;
        this.sessionInitialized = true;
        bumpApiClientQueryScope(apiClient);
      }
    });
    const initialTokens = await tokenStorage.loadTokens();
    if (!this.sessionInitialized) {
      this.sessionIdentity = resolveSessionIdentity(initialTokens);
      this.sessionInitialized = true;
    }
  }

  dispose() {
    this.unsubscribeTokens?.();
    this.unsubscribeTokens = undefined;
    this.sessionInitialized = false;
  }

  getRouteImports(): RequireContext[] {
    // @ts-ignore
    return [require.context('./routes')];
  }

  getScreens(): ScreenData[] {
    return [
      {
        route: 'oauthredirect',
        label: 'OAuth',
        hideInNav: true,
      },
      {
        route: 'oauthredirect/index',
        label: 'OAuth',
        hideInNav: true,
      },
    ];
  }

  wrapProvider(children: ReactNode): ReactNode {
    return <ProtectedRoute>{children}</ProtectedRoute>;
  }
}


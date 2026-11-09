import type {SessionProvider} from '@tmobis/core/auth/session-provider';
import tokenStorage, {subscribeToTokens} from './storage';

export const keycloakSessionProvider: SessionProvider = {
  async getSession() {
    return toAuthSession(
      (await tokenStorage.loadTokens())?.backendAccessToken?.accessToken
    );
  },
  subscribe: (listener) => subscribeToTokens((tokens) => {
    listener(toAuthSession(tokens?.backendAccessToken?.accessToken));
  }),
};

function toAuthSession(accessToken?: string) {
  return accessToken ? {accessToken} : undefined;
}

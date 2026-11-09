import { StoredTokens, TokenStorage } from '../types';

const STORAGE_KEY = 'keycloak.tokens';

export default {
  async saveTokens(tokens: StoredTokens): Promise<void> {
    assertBrowserEnvironment();
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
  },

  async loadTokens(): Promise<StoredTokens | null> {
    assertBrowserEnvironment();
    const json = localStorage.getItem(STORAGE_KEY);
    if (!json) return null;

    try {
      const parsed = JSON.parse(json) as unknown;
      if (!isValidStoredTokens(parsed)) {
        throw new Error('Invalid token structure');
      }
      return parsed;
    } catch (error) {
      throw new Error('storage_corrupted');
    }
  },

  async clearTokens(): Promise<void> {
    assertBrowserEnvironment();
    localStorage.removeItem(STORAGE_KEY);
  },
};

function assertBrowserEnvironment(): void {
  if (typeof window === 'undefined' || typeof localStorage === 'undefined') {
    throw new Error('localStorage is not available in this environment');
  }
}

function isValidStoredTokens(value: unknown): value is StoredTokens {
  if (typeof value !== 'object' || value === null) return false;
  const t = value as Record<string, unknown>;
  return (
    typeof t.accessToken === 'string' &&
    typeof t.idToken === 'string' &&
    typeof t.backendAccessToken === 'object'
  );
}
import * as SecureStore from 'expo-secure-store';
import { StoredTokens, TokenStorage } from '../types';

const STORAGE_KEY = 'keycloak.tokens';

export default {
  async saveTokens(tokens: StoredTokens): Promise<void> {
    await SecureStore.setItemAsync(STORAGE_KEY, JSON.stringify(tokens));
  },

  async loadTokens(): Promise<StoredTokens | null> {
    const json = await SecureStore.getItemAsync(STORAGE_KEY);
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
    await SecureStore.deleteItemAsync(STORAGE_KEY);
  },
};

function isValidStoredTokens(value: unknown): value is StoredTokens {
  if (typeof value !== 'object' || value === null) return false;
  const t = value as Record<string, unknown>;
  return (
    typeof t.accessToken === 'string' &&
    typeof t.idToken === 'string' &&
    typeof t.backendAccessToken === 'object'
  );
}
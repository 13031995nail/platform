import {Platform} from "react-native";
import nativeTokenStorage from './storage.native';
import webTokenStorage from './storage.web';
import type {StoredTokens, TokenStorage} from '../types';

type TokenListener = (tokens: Readonly<StoredTokens> | null) => void;

const platformTokenStorage = Platform.OS === 'web'
  ? webTokenStorage
  : nativeTokenStorage;
const tokenListeners = new Set<TokenListener>();

const tokenStorage: TokenStorage = {
  async saveTokens(tokens) {
    await platformTokenStorage.saveTokens(tokens);
    notifyTokenListeners(tokens);
  },
  loadTokens: () => platformTokenStorage.loadTokens(),
  async clearTokens() {
    await platformTokenStorage.clearTokens();
    notifyTokenListeners(null);
  },
};

export function subscribeToTokens(listener: TokenListener): () => void {
  tokenListeners.add(listener);
  return () => tokenListeners.delete(listener);
}

function notifyTokenListeners(tokens: Readonly<StoredTokens> | null): void {
  for (const listener of tokenListeners) {
    try {
      listener(tokens);
    } catch {
      // Ошибка подписчика не должна влиять на сохранение токенов.
    }
  }
}

export type {TokenStorage} from '../types';
export default tokenStorage;

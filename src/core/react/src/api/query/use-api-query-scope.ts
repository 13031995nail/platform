import { useCallback, useSyncExternalStore } from 'react';
import { useApiClient } from '../api-client-context';
import {
  getApiClientQueryKey,
  getApiClientQueryRevision,
  subscribeApiClientQueryScope,
} from './query-scope';

/** Реактивный scope: при смене токенов React-хуки получают новые queryKey. */
export function useApiQueryScope() {
  const client = useApiClient();
  const subscribe = useCallback(
    (listener: () => void) => subscribeApiClientQueryScope(client, listener),
    [client]
  );
  const snapshot = useCallback(() => getApiClientQueryRevision(client), [client]);
  const revision = useSyncExternalStore(subscribe, snapshot, snapshot);
  const makeQueryKey = useCallback(
    (key: readonly unknown[]) => getApiClientQueryKey(client, key),
    [client, revision]
  );
  return { client, revision, makeQueryKey };
}

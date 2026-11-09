import type { ApiClient } from '../create-api-client';

type Scope = { id: number; revision: number; listeners: Set<() => void> };
const scopes = new WeakMap<ApiClient, Scope>();
const cacheListeners = new Set<(clientId: number, revision: number) => void>();
let lastId = 0;

function getScope(client: ApiClient): Scope {
  let scope = scopes.get(client);
  if (!scope) {
    scope = { id: ++lastId, revision: 0, listeners: new Set() };
    scopes.set(client, scope);
  }
  return scope;
}

/** Ключи разделены по экземпляру HTTP-клиента и сеансу авторизации. */
export function getApiClientQueryKey(client: ApiClient, key: readonly unknown[]): readonly unknown[] {
  const { id, revision } = getScope(client);
  return ['api-client', id, revision, ...key];
}

export function getApiClientQueryRevision(client: ApiClient): number {
  return getScope(client).revision;
}

export function subscribeApiClientQueryScope(client: ApiClient, listener: () => void): () => void {
  const scope = getScope(client);
  scope.listeners.add(listener);
  return () => { scope.listeners.delete(listener); };
}

/** QueryProvider очищает данные только изменённого клиента. */
export function subscribeApiClientCacheInvalidation(
  listener: (clientId: number, revision: number) => void
): () => void {
  cacheListeners.add(listener);
  return () => { cacheListeners.delete(listener); };
}

/** Обновление токенов приводит к смене ключей и уведомлению всех подписчиков. */
export function bumpApiClientQueryScope(client: ApiClient): void {
  const scope = getScope(client);
  const previousRevision = scope.revision;
  scope.revision++;
  for (const listener of cacheListeners) listener(scope.id, previousRevision);
  for (const listener of scope.listeners) listener();
}

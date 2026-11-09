import assert from 'node:assert/strict';
import { test } from 'node:test';
import { createApiClient } from '../create-api-client';
import {
  bumpApiClientQueryScope,
  getApiClientQueryKey,
  getApiClientQueryRevision,
  subscribeApiClientQueryScope,
  subscribeApiClientCacheInvalidation,
} from './query-scope';

test('Кеш разных клиентов изолирован и изменение токена уведомляет React-подписчиков', () => {
  const a = createApiClient();
  const b = createApiClient();
  const previous = getApiClientQueryKey(a, ['Employee', 1]);
  assert.notDeepEqual(previous, getApiClientQueryKey(b, ['Employee', 1]));
  let notifications = 0;
  let invalidatedClientId: number | undefined;
  let invalidatedRevision: number | undefined;
  const unsubscribe = subscribeApiClientQueryScope(a, () => { notifications++; });
  const stop = subscribeApiClientCacheInvalidation((id, revision) => { invalidatedClientId = id; invalidatedRevision = revision; });

  bumpApiClientQueryScope(a);

  assert.equal(notifications, 1);
  assert.equal(invalidatedClientId, previous[1]);
  assert.equal(invalidatedRevision, previous[2]);
  assert.equal(getApiClientQueryRevision(a), 1);
  assert.equal(getApiClientQueryRevision(b), 0);
  assert.notDeepEqual(previous, getApiClientQueryKey(a, ['Employee', 1]));
  unsubscribe();
  stop();
  bumpApiClientQueryScope(a);
  assert.equal(notifications, 1);
});

test('Отмена старого запроса не затрагивает ключ новой ревизии', async () => {
  const { QueryClient } = await import('@tanstack/react-query');
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const api = createApiClient();
  const oldKey = getApiClientQueryKey(api, ['old-request']);
  const cancellation: Array<Promise<void>> = [];
  let aborted = false;

  const unsubscribe = subscribeApiClientCacheInvalidation((id, previousRevision) => {
    const queryKey = ['api-client', id, previousRevision];
    const operation = queryClient.cancelQueries({ queryKey }).then(() => {
      queryClient.removeQueries({ queryKey });
    });
    cancellation.push(operation);
  });

  const request = queryClient.fetchQuery({
    queryKey: oldKey,
    queryFn: ({ signal }) => new Promise<string>((_resolve, reject) => {
      signal.addEventListener('abort', () => {
        aborted = true;
        reject(new Error('canceled'));
      }, { once: true });
    }),
  });
  bumpApiClientQueryScope(api);
  await assert.rejects(request);
  await Promise.all(cancellation);

  assert.equal(aborted, true);
  assert.equal(queryClient.getQueryState(oldKey), undefined);
  const newKey = getApiClientQueryKey(api, ['new-request']);
  assert.notDeepEqual(newKey.slice(0, 3), oldKey.slice(0, 3));
  unsubscribe();
});

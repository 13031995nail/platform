import assert from 'node:assert/strict';
import { test } from 'node:test';
import { AxiosError, AxiosHeaders } from 'axios';
import { ApiError } from './api-error';
import {
  createApiClient,
  configureApiAuth,
} from './create-api-client';
import { getApiClientQueryKey, bumpApiClientQueryScope } from './query/query-scope';

test('each API client carries its own auth callback and endpoint', async () => {
  const headers: string[] = [];
  const adapter = async (config: import('axios').InternalAxiosRequestConfig) => {
    headers.push(String(config.headers.get('Authorization')));
    return {
      data: config.baseURL,
      status: 200,
      statusText: 'OK',
      headers: new AxiosHeaders(),
      config,
    };
  };
  const a = createApiClient({ baseURL: 'https://a.test', adapter, getAccessToken: async () => 'a-token' });
  const b = createApiClient({ baseURL: 'https://b.test', adapter });
  configureApiAuth(b, async () => 'b-token');
  assert.equal((await a.get('/test')).data, 'https://a.test');
  assert.equal((await b.get('/test')).data, 'https://b.test');
  assert.deepEqual(headers, ['Bearer a-token', 'Bearer b-token']);
});

test('API response errors are normalized without rewriting AxiosError', async () => {
  const a = createApiClient({
    adapter: async config => {
      const response = {
        data: { code: 'DENIED', message: 'Not allowed' },
        status: 403,
        statusText: 'Forbidden',
        headers: new AxiosHeaders({ 'x-correlation-id': 'abc-123' }),
        config,
      };
      throw new AxiosError('Request failed', 'ERR_BAD_REQUEST', config, undefined, response);
    },
  });
  await assert.rejects(a.get('/test'), err => {
    assert.ok(err instanceof ApiError);
    assert.equal(err.status, 403);
    assert.equal(err.code, 'DENIED');
    assert.equal(err.correlationId, 'abc-123');
    assert.equal((err.original as AxiosError).message, 'Request failed');
    return true;
  });
});

test('query keys isolate clients and rotate when credentials change', () => {
  const a = createApiClient();
  const b = createApiClient();
  const first = getApiClientQueryKey(a, ['entities', 1]);
  assert.deepEqual(first, getApiClientQueryKey(a, ['entities', 1]));
  assert.notDeepEqual(first, getApiClientQueryKey(b, ['entities', 1]));
  bumpApiClientQueryScope(a);
  assert.notDeepEqual(first, getApiClientQueryKey(a, ['entities', 1]));
});

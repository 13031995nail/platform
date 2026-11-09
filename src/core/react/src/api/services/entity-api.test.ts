import assert from 'node:assert/strict';
import { test } from 'node:test';
import { AxiosHeaders, type InternalAxiosRequestConfig } from 'axios';
import { createApiClient } from '../create-api-client';
import { createEntityApi } from './entity-api';

test('Фабрики сущностей направляют запросы на назначенные endpoints', async () => {
  const requests: Array<{ baseURL?: string; path?: string }> = [];
  const adapter = async (config: InternalAxiosRequestConfig) => {
    requests.push({ baseURL: config.baseURL, path: config.url });
    return {
      data: { id: '1' },
      status: 200,
      statusText: 'OK',
      headers: new AxiosHeaders(),
      config,
    };
  };
  const a = createEntityApi(createApiClient({ baseURL: 'https://one.test', adapter }));
  const b = createEntityApi(createApiClient({ baseURL: 'https://two.test', adapter }));
  await a.getEntity('1', 'Employee');
  await b.getEntity('1', 'Employee');
  assert.deepEqual(requests, [
    { baseURL: 'https://one.test', path: '/api/Employee/1?' },
    { baseURL: 'https://two.test', path: '/api/Employee/1?' },
  ]);
});

test('AbortSignal из React Query доходит до Axios', async () => {
  const controller = new AbortController();
  let receivedSignal: InternalAxiosRequestConfig['signal'];
  const adapter = async (config: InternalAxiosRequestConfig) => {
    receivedSignal = config.signal;
    return { data: { id: '1' }, status: 200, statusText: 'OK', headers: new AxiosHeaders(), config };
  };
  const service = createEntityApi(createApiClient({ adapter }));
  await service.getEntity('1', 'Employee', undefined, controller.signal);
  assert.strictEqual(receivedSignal, controller.signal);
});

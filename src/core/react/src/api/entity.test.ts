import assert from 'node:assert/strict';
import { test } from 'node:test';
import { AxiosHeaders, type InternalAxiosRequestConfig } from 'axios';
import { createApiClient } from './create-api-client';
import { createEntityApi } from './services/entity-api';

test('entity API uses POST to create and PUT /{id} to update', async () => {
  const calls: { method?: string; url?: string; data?: string }[] = [];
  const adapter = async (config: InternalAxiosRequestConfig) => {
    calls.push({ method: config.method, url: config.url, data: config.data });
    return {
      config,
      data: { id: '123e4567-e89b-12d3-a456-426614174000' },
      headers: new AxiosHeaders(),
      status: 200,
      statusText: 'OK',
    };
  };
  const service = createEntityApi(createApiClient({ adapter }));
  await service.createEntity({ id: undefined, name: 'New' }, 'Example');
  const id = '123e4567-e89b-12d3-a456-426614174001';
  await service.updateEntity({ id: undefined, name: 'Changed' }, 'Example', id);

  assert.deepEqual(
    calls.map(({ method, url }) => ({ method, url })),
    [
      { method: 'post', url: '/api/Example' },
      { method: 'put', url: `/api/Example/${id}` },
    ],
  );
  assert.equal(JSON.parse(calls[1].data!).name, 'Changed');
  assert.equal(JSON.parse(calls[1].data!).id, undefined);
});

test('updateEntity refuses to PUT without an id', () => {
  const service = createEntityApi(createApiClient());
  assert.throws(
    () => service.updateEntity({ id: undefined }, 'Example'),
    /Не указан идентификатор/,
  );
  assert.throws(
    () => service.updateEntity({ id: undefined }, 'Example', ''),
    /Не указан идентификатор/,
  );
});

test('getEntities preserves the PagedResult contract and includeTotalCount', async () => {
  const calls: string[] = [];
  const adapter = async (config: InternalAxiosRequestConfig) => {
    calls.push(config.url ?? '');
    return {
      config,
      data: { items: [{ id: 'entity-id' }], totalCount: 12 },
      headers: new AxiosHeaders(),
      status: 200,
      statusText: 'OK',
    };
  };
  const service = createEntityApi(createApiClient({ adapter }));
  const response = await service.getEntities({ entityType: 'Example', includeTotalCount: true });
  assert.deepEqual(response, { items: [{ id: 'entity-id' }], totalCount: 12 });
  assert.match(calls[0], /includeTotalCount=true/);
});

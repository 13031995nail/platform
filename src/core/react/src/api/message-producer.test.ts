import assert from 'node:assert/strict';
import { test } from 'node:test';
import { Message, type MessageMetadata } from '../models/message';
import { createApiClient } from './create-api-client';
import { createMessageApi } from './services/message-api';
import { createWebApiMessageProducer } from './message-producer';
import { AxiosHeaders, type InternalAxiosRequestConfig } from 'axios';

class TestMessage extends Message<{ received: boolean }> {
  static metadata = { type: 'query', group: 'Tests', name: 'Example' } satisfies MessageMetadata;
}

test('WebApiMessageProducer sends via the injected API service', async () => {
  const endpoints: string[] = [];
  const adapter = async (config: InternalAxiosRequestConfig) => {
    endpoints.push(config.url ?? '');
    return {
      data: { received: true },
      status: 200,
      statusText: 'OK',
      headers: new AxiosHeaders(),
      config,
    };
  };
  const client = createApiClient({ baseURL: 'https://api.example.test', adapter });
  const producer = createWebApiMessageProducer(createMessageApi(client));
  const result = await producer.send(new TestMessage(), TestMessage.metadata);
  assert.deepEqual(result, { received: true });
  assert.deepEqual(endpoints, ['/api/Tests/Example']);
});

import type { ApiClient } from '../create-api-client';
import {
  getMessageMetadata,
  InferMessageResult,
  IMessage,
  MessageMetadata,
} from '../../models/message';

const EXCLUDED_MESSAGE_KEYS = new Set(['context', '__resultType']);

const isRecord = (value: unknown): value is Record<string, unknown> => {
  if (value === null || typeof value !== 'object') {
    return false;
  }

  const prototype = Object.getPrototypeOf(value);
  return prototype === Object.prototype || prototype === null;
};

const normalizeValue = (value: unknown): unknown => {
  if (value === undefined) {
    return undefined;
  }

  if (Array.isArray(value)) {
    return value.map((item) => normalizeValue(item));
  }

  if (!isRecord(value)) {
    return value;
  }

  const normalized: Record<string, unknown> = {};
  for (const key of Object.keys(value).sort()) {
    const normalizedItem = normalizeValue(value[key]);
    if (normalizedItem !== undefined) {
      normalized[key] = normalizedItem;
    }
  }

  return normalized;
};

export const normalizeMessage = (
  message: IMessage
): Record<string, unknown> => {
  const plain = { ...message } as Record<string, unknown>;
  const normalized: Record<string, unknown> = {};

  for (const key of Object.keys(plain).sort()) {
    if (EXCLUDED_MESSAGE_KEYS.has(key)) {
      continue;
    }

    const normalizedItem = normalizeValue(plain[key]);
    if (normalizedItem !== undefined) {
      normalized[key] = normalizedItem;
    }
  }

  return normalized;
};

export const getMessageEndpoint = (metadata: MessageMetadata): string => {
  return `/api/${metadata.group ? `${metadata.group}/` : ''}${metadata.name}`;
};

export const serializeMessageQuery = (message: IMessage): URLSearchParams => {
  return new URLSearchParams({
    messageJson: JSON.stringify(normalizeMessage(message)),
  });
};

export const getMessageQueryKey = (
  message: IMessage,
  metadata = getMessageMetadata(message)
) => {
  return [
    'message',
    metadata.group ?? '',
    metadata.name,
    normalizeMessage(message),
  ] as const;
};

export const sendMessage = async <
  TMessage extends IMessage,
  TMessageResult = InferMessageResult<TMessage>
>(
  message: TMessage,
  metadata: MessageMetadata,
  client: ApiClient,
  signal?: AbortSignal
): Promise<TMessageResult> => {
  const endpoint = getMessageEndpoint(metadata);
  if (metadata.type === 'query') {
    const query = serializeMessageQuery(message);
    const response = await client.get<TMessageResult>(endpoint, { params: query, signal });
    return response.data;
  }

  const response = await client.post<TMessageResult>(endpoint, message, { signal });
  return response.data;
};

/** Фабрика с внедрённым клиентом. */
export function createMessageApi(client: ApiClient) {
  return {
    sendMessage: <T extends IMessage, R = InferMessageResult<T>>(
      message: T, metadata: MessageMetadata = getMessageMetadata(message), signal?: AbortSignal
    ): Promise<R> => sendMessage<T, R>(message, metadata, client, signal),
  };
}
export type MessageApi = ReturnType<typeof createMessageApi>;

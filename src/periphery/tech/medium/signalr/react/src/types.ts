import type SignalRClient from './client';

export interface SignalRCallbackContext {
  type: string;
  client: SignalRClient;
}

export type SignalRMessageHandler<TPayload = unknown> = (
  payload: TPayload,
  context: SignalRCallbackContext
) => void | Promise<void>;

export interface SignalRCallbackError {
  error: unknown;
  type: string;
  scope: 'global' | 'typed';
}

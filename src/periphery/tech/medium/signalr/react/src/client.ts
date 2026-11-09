import {
  HubConnectionBuilder,
  HubConnectionState,
  type IHttpConnectionOptions,
} from '@microsoft/signalr';
import {
  getMessageMetadata,
  type IMessage,
  type MessageMetadata,
} from '@tmobis/core/models/message';
import SignalRCallbackRegistry from './registry';
import type {
  SignalRCallbackError,
  SignalRMessageHandler,
} from './types';

const defaultReceiveMethod = 'ReceiveMessage';
const defaultInitialRetryDelayMs = 5_000;
const minimumRetryDelayMs = 1_000;
const maximumRetryDelayMs = 60_000;
const maximumReconnectDelayCount = 10;
const retryJitterRatio = 0.2;

export interface SignalRConnection {
  readonly state: HubConnectionState;
  start(): Promise<void>;
  stop(): Promise<void>;
  invoke<T = unknown>(methodName: string, ...args: unknown[]): Promise<T>;
  on(methodName: string, handler: (...args: unknown[]) => void): void;
  off(methodName: string, handler: (...args: unknown[]) => void): void;
  onclose(callback: (error?: Error) => void): void;
}

export interface SignalRConnectionFactoryOptions {
  url: string;
  accessTokenFactory: NonNullable<IHttpConnectionOptions['accessTokenFactory']>;
  reconnectDelays?: readonly number[];
  automaticReconnect: boolean;
}

export type SignalRConnectionFactory = (
  options: SignalRConnectionFactoryOptions
) => SignalRConnection;

export type SignalROutgoingMessage = IMessage & {
  readonly $type: string;
};

type SignalROutgoingMessageSelection =
  | {accepted: true; type: string}
  | {accepted: false; error: string};

export interface SignalRClientOptions {
  url: string;
  accessTokenFactory: NonNullable<IHttpConnectionOptions['accessTokenFactory']>;
  receiveMethod?: string;
  reconnectDelays?: readonly number[];
  automaticReconnect?: boolean;
  initialRetryDelayMs?: number;
  connectionFactory?: SignalRConnectionFactory;
  onError?: (error: unknown) => void;
  onCallbackError?: (details: SignalRCallbackError) => void;
}

export default class SignalRClient {
  private readonly connection: SignalRConnection;
  private readonly registry: SignalRCallbackRegistry;
  private readonly receiveMethod: string;
  private readonly initialRetryDelayMs: number;
  private readonly eventHandlers = new Map<
    string,
    (...args: unknown[]) => void
  >();
  private shouldBeStarted = false;
  private transition: Promise<void> = Promise.resolve();
  private retryTimer?: ReturnType<typeof setTimeout>;
  private retryAttempt = 0;

  constructor(private readonly options: SignalRClientOptions) {
    if (!options.url.trim()) throw new Error('SignalR URL is required');

    this.receiveMethod = options.receiveMethod ?? defaultReceiveMethod;
    this.initialRetryDelayMs = normalizeRetryDelay(
      options.initialRetryDelayMs
    );
    this.registry = new SignalRCallbackRegistry(options.onCallbackError);
    this.connection = (options.connectionFactory ?? createConnection)({
      url: options.url,
      accessTokenFactory: options.accessTokenFactory,
      reconnectDelays: normalizeReconnectDelays(options.reconnectDelays),
      automaticReconnect: options.automaticReconnect ?? true,
    });

    const receiveHandler = (...args: unknown[]) => {
      void this.receiveUniversal(args);
    };
    this.eventHandlers.set(this.receiveMethod, receiveHandler);
    this.connection.on(this.receiveMethod, receiveHandler);
    this.connection.onclose((error) => {
      if (error) this.reportError(error);
      this.scheduleRetry();
    });
  }

  start(): Promise<void> {
    this.shouldBeStarted = true;
    return this.enqueueTransition();
  }

  stop(): Promise<void> {
    this.shouldBeStarted = false;
    this.clearRetry();
    this.retryAttempt = 0;
    return this.enqueueTransition();
  }

  dispose(): Promise<void> {
    return this.stop();
  }

  canSend(message: IMessage, metadata: MessageMetadata): boolean {
    return selectSignalROutgoingMessage(message, metadata).accepted;
  }

  async send<TMessage extends SignalROutgoingMessage>(
    message: TMessage,
    metadata: MessageMetadata = getMessageMetadata(message)
  ): Promise<void> {
    const selection = selectSignalROutgoingMessage(message, metadata);
    if (!selection.accepted) {
      throw new Error(selection.error);
    }
    if (selection.type.includes(',')) {
      throw new Error(
        'SignalR message $type must be a full type name without assembly'
      );
    }
    await this.connection.invoke<void>(
      'SendMessage',
      JSON.stringify({...message, $type: selection.type})
    );
  }

  on<TPayload>(
    type: string,
    callback: SignalRMessageHandler<TPayload>
  ): () => void {
    const wasRegistered = this.registry.has(type);
    const unregister = this.registry.on(type, callback);
    if (!wasRegistered) this.attachTypedEvent(type);

    return () => {
      unregister();
      if (!this.registry.has(type)) this.detachTypedEvent(type);
    };
  }

  onGlobal(callback: SignalRMessageHandler<unknown>): () => void {
    return this.registry.onGlobal(callback);
  }

  private enqueueTransition(): Promise<void> {
    const transition = this.transition.then(
      () => this.reconcileConnection(),
      () => this.reconcileConnection()
    );
    this.transition = transition.catch(() => undefined);
    return transition;
  }

  private async reconcileConnection(): Promise<void> {
    if (this.shouldBeStarted) {
      if (this.connection.state !== HubConnectionState.Disconnected) return;
      try {
        await this.connection.start();
        this.clearRetry();
        this.retryAttempt = 0;
      } catch (error) {
        this.reportError(error);
        this.scheduleRetry();
        throw error;
      }
      return;
    }

    if (this.connection.state !== HubConnectionState.Disconnected) {
      await this.connection.stop();
    }
  }

  private attachTypedEvent(type: string): void {
    if (type === this.receiveMethod || this.eventHandlers.has(type)) return;

    const handler = (rawData: unknown) => {
      void this.dispatchIncoming(rawData, type);
    };
    this.eventHandlers.set(type, handler);
    this.connection.on(type, handler);
  }

  private detachTypedEvent(type: string): void {
    if (type === this.receiveMethod) return;
    const handler = this.eventHandlers.get(type);
    if (!handler) return;
    this.connection.off(type, handler);
    this.eventHandlers.delete(type);
  }

  private async receiveUniversal(args: readonly unknown[]): Promise<void> {
    const explicitType = typeof args[0] === 'string' && args.length > 1
      ? args[0]
      : undefined;
    const rawData = explicitType ? args[1] : args[0];
    await this.dispatchIncoming(rawData, explicitType);
  }

  private async dispatchIncoming(
    rawData: unknown,
    fallbackType?: string
  ): Promise<void> {
    const payload = parseJsonIfPossible(rawData);
    const type = fallbackType ?? getMessageType(payload);
    if (!type) {
      this.reportError(new Error('SignalR message type is missing'));
      return;
    }
    await this.registry.dispatch(type, payload, this);
  }

  private scheduleRetry(): void {
    if (!this.shouldBeStarted || this.retryTimer) return;
    const exponentialDelay = Math.min(
      maximumRetryDelayMs,
      this.initialRetryDelayMs * (2 ** Math.min(this.retryAttempt, 10))
    );
    const jitter = exponentialDelay * retryJitterRatio
      * ((Math.random() * 2) - 1);
    const delay = Math.min(
      maximumRetryDelayMs,
      Math.max(minimumRetryDelayMs, Math.round(exponentialDelay + jitter))
    );
    this.retryAttempt += 1;
    this.retryTimer = setTimeout(() => {
      this.retryTimer = undefined;
      void this.start().catch(() => undefined);
    }, delay);
  }

  private clearRetry(): void {
    if (!this.retryTimer) return;
    clearTimeout(this.retryTimer);
    this.retryTimer = undefined;
  }

  private reportError(error: unknown): void {
    try {
      this.options.onError?.(error);
    } catch {
      // Ошибка обработчика ошибок не должна влиять на подключение.
    }
  }
}

function createConnection(
  options: SignalRConnectionFactoryOptions
): SignalRConnection {
  let builder = new HubConnectionBuilder().withUrl(options.url, {
    accessTokenFactory: options.accessTokenFactory,
  });
  if (options.automaticReconnect) {
    builder = options.reconnectDelays
      ? builder.withAutomaticReconnect([...options.reconnectDelays])
      : builder.withAutomaticReconnect();
  }
  return builder.build();
}

function parseJsonIfPossible(rawData: unknown): unknown {
  if (typeof rawData !== 'string') return rawData;
  try {
    return JSON.parse(rawData) as unknown;
  } catch {
    return rawData;
  }
}

function getMessageType(payload: unknown): string | undefined {
  if (!payload || typeof payload !== 'object') return undefined;
  const message = payload as Record<string, unknown>;
  const type = message.$type;
  return typeof type === 'string' && type ? type : undefined;
}

function selectSignalROutgoingMessage(
  message: IMessage,
  metadata: MessageMetadata
): SignalROutgoingMessageSelection {
  const type = (message as Partial<SignalROutgoingMessage>).$type;
  if (typeof type !== 'string' || !type.trim()) {
    return {
      accepted: false,
      error: 'SignalR message $type is required',
    };
  }
  if (metadata.type !== 'event') {
    return {
      accepted: false,
      error: `SignalR send supports only event messages; received "${metadata.type}"`,
    };
  }
  return {
    accepted: true,
    type: type.trim(),
  };
}

function normalizeRetryDelay(delay: number | undefined): number {
  if (delay === undefined || !Number.isFinite(delay)) {
    return defaultInitialRetryDelayMs;
  }
  return Math.min(
    maximumRetryDelayMs,
    Math.max(minimumRetryDelayMs, Math.round(delay))
  );
}

export function normalizeReconnectDelays(
  delays: readonly number[] | undefined
): readonly number[] | undefined {
  if (delays === undefined) return undefined;

  return delays
    .slice(0, maximumReconnectDelayCount)
    .filter(Number.isFinite)
    .map((delay) => Math.min(
      maximumRetryDelayMs,
      Math.max(minimumRetryDelayMs, Math.round(delay))
    ));
}

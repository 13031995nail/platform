import type SignalRClient from './client';
import type {
  SignalRCallbackContext,
  SignalRCallbackError,
  SignalRMessageHandler,
} from './types';

type CallbackErrorHandler = (details: SignalRCallbackError) => void;
type CallbackRegistration = {
  callback: SignalRMessageHandler<unknown>;
};

export default class SignalRCallbackRegistry {
  private readonly callbacks = new Map<
    string,
    Set<CallbackRegistration>
  >();
  private globalCallback?: CallbackRegistration;

  constructor(
    private readonly onCallbackError?: CallbackErrorHandler
  ) {}

  on<TPayload>(
    type: string,
    callback: SignalRMessageHandler<TPayload>
  ): () => void {
    if (!type.trim()) throw new Error('SignalR message type is required');

    let callbacks = this.callbacks.get(type);
    if (!callbacks) {
      callbacks = new Set();
      this.callbacks.set(type, callbacks);
    }
    const registration: CallbackRegistration = {
      callback: callback as SignalRMessageHandler<unknown>,
    };
    callbacks.add(registration);

    return () => {
      callbacks?.delete(registration);
      if (callbacks?.size === 0) this.callbacks.delete(type);
    };
  }

  onGlobal(callback: SignalRMessageHandler<unknown>): () => void {
    const registration: CallbackRegistration = {callback};
    this.globalCallback = registration;

    return () => {
      if (this.globalCallback === registration) {
        this.globalCallback = undefined;
      }
    };
  }

  has(type: string): boolean {
    return (this.callbacks.get(type)?.size ?? 0) > 0;
  }

  async dispatch(
    type: string,
    payload: unknown,
    client: SignalRClient
  ): Promise<void> {
    const context: SignalRCallbackContext = {
      type,
      client,
    };

    await this.invokeCallbacks(
      this.globalCallback ? [this.globalCallback.callback] : [],
      payload,
      context,
      'global'
    );
    await this.invokeCallbacks(
      [...(this.callbacks.get(type) ?? [])].map(
        (registration) => registration.callback
      ),
      payload,
      context,
      'typed'
    );
  }

  private async invokeCallbacks(
    callbacks: readonly SignalRMessageHandler<unknown>[],
    payload: unknown,
    context: SignalRCallbackContext,
    scope: SignalRCallbackError['scope']
  ): Promise<void> {
    for (const callback of callbacks) {
      try {
        await callback(payload, context);
      } catch (error) {
        try {
          this.onCallbackError?.({error, type: context.type, scope});
        } catch {
          // Ошибка обработчика ошибок не должна прерывать доставку сообщения.
        }
      }
    }
  }
}

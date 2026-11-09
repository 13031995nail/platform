import type {SessionProvider} from '@tmobis/core/auth/session-provider';
import SignalRClient, {type SignalRClientOptions} from './client';
import type {SignalRMessageHandler} from './types';
import Constants from 'expo-constants';

export interface SignalRConnectionOptions extends Omit<
  SignalRClientOptions,
  'accessTokenFactory' | 'url'
> {
  onLifecycleError?: (event: SignalRLifecycleErrorEvent) => void;
}

export interface SignalRServiceOptions extends SignalRConnectionOptions {
  sessionProvider: SessionProvider;
}

export type SignalRLifecycleErrorEvent =
  | 'connection-stop-failed'
  | 'gate-reconciliation-failed';

const initialStopRetryDelayMs = 250;
const maximumStopRetryDelayMs = 30_000;
const stopAttemptTimeoutMs = 30_000;

type StopAttemptResult = 'succeeded' | 'failed' | 'cancelled';

function getSignalRUrl(): string {
  const extra = Constants.expoConfig?.extra;
  if (typeof extra?.signalrUrl === 'string') return extra.signalrUrl;

  const backendUrl = String(extra?.backendUrl ?? '').replace(/\/$/, '');
  return `${backendUrl}/signalr`;
}

export default class SignalRService {
  readonly client: SignalRClient;
  private readonly sessionProvider: SessionProvider;
  private gateUnsubscribe?: () => void;
  private isActive = false;
  private gateTransition: Promise<void> = Promise.resolve();
  private gateRevision = 0;
  private pendingStopWait?: {cancel: () => void};
  private readonly onLifecycleError?: (
    event: SignalRLifecycleErrorEvent
  ) => void;

  constructor(options: SignalRServiceOptions) {
    const {
      sessionProvider,
      onLifecycleError,
      ...clientOptions
    } = options;
    this.sessionProvider = sessionProvider;
    this.onLifecycleError = onLifecycleError;
    this.client = new SignalRClient({
      ...clientOptions,
      url: getSignalRUrl(),
      accessTokenFactory: async () => {
        const gateRevision = this.gateRevision;
        const session = await this.sessionProvider.getSession();
        if (this.gateRevision !== gateRevision) {
          throw new Error(
            'SignalR session changed while resolving the access token'
          );
        }
        const token = session?.accessToken;
        if (!token) throw new Error('Access token is missing');
        return token;
      },
    });
  }

  on<TPayload>(
    type: string,
    callback: SignalRMessageHandler<TPayload>
  ): () => void {
    return this.client.on(type, callback);
  }

  onGlobal(callback: SignalRMessageHandler<unknown>): () => void {
    return this.client.onGlobal(callback);
  }

  async initialize(): Promise<void> {
    this.isActive = true;
    if (!this.gateUnsubscribe) {
      this.gateUnsubscribe = this.sessionProvider.subscribe(() => {
        this.requestGateReconciliation(true);
      });
    }
    const revision = ++this.gateRevision;
    await this.enqueueGateReconciliation(false, revision);
  }

  async dispose(): Promise<void> {
    this.isActive = false;
    const revision = ++this.gateRevision;
    this.cancelPendingStopWait();
    this.gateUnsubscribe?.();
    this.gateUnsubscribe = undefined;
    await this.enqueueGateReconciliation(false, revision);
  }

  private requestGateReconciliation(restart: boolean): void {
    const revision = ++this.gateRevision;
    this.cancelPendingStopWait();
    void this.enqueueGateReconciliation(restart, revision).catch(() => {
      this.reportLifecycleError('gate-reconciliation-failed');
    });
  }

  private enqueueGateReconciliation(
    restart: boolean,
    revision: number
  ): Promise<void> {
    const transition = this.gateTransition.then(
      () => this.reconcileConnectionGate(restart, revision),
      () => this.reconcileConnectionGate(restart, revision)
    );
    this.gateTransition = transition.catch(() => undefined);
    return transition;
  }

  private async reconcileConnectionGate(
    restart: boolean,
    revision: number
  ): Promise<void> {
    if (revision !== this.gateRevision) return;
    const gateOpen = await this.isConnectionAllowed();
    if (revision !== this.gateRevision) return;
    if (!gateOpen) {
      await this.stopWithRetry(revision);
      return;
    }

    if (restart) {
      await this.stopWithRetry(revision);
      if (revision !== this.gateRevision || !this.isActive) return;
      const gateStillOpen = await this.isConnectionAllowed();
      if (
        revision !== this.gateRevision
        || !this.isActive
        || !gateStillOpen
      ) return;
    }

    if (revision !== this.gateRevision || !this.isActive) return;
    try {
      await this.client.start();
    } catch {
      // Клиент продолжает попытки подключения в фоне.
    } finally {
      if (revision !== this.gateRevision || !this.isActive) {
        await this.stopStaleStart();
      }
    }
  }

  private async stopStaleStart(): Promise<void> {
    // Согласование состояния выполняется последовательно до конца очистки,
    // поэтому остановка завершится до обработки следующего изменения.
    await this.stopWithRetry(this.gateRevision);
  }

  private async stopWithRetry(revision: number): Promise<void> {
    let retryAttempt = 0;
    while (revision === this.gateRevision) {
      const stopResult = await this.waitForStopAttempt(revision);
      if (stopResult === 'succeeded') return;
      if (stopResult === 'cancelled') return;
      this.reportLifecycleError('connection-stop-failed');

      if (revision !== this.gateRevision) return;
      const retryDelay = Math.min(
        maximumStopRetryDelayMs,
        initialStopRetryDelayMs * (2 ** Math.min(retryAttempt, 16))
      );
      retryAttempt += 1;
      const shouldContinue = await this.waitForStopRetry(
        retryDelay,
        revision
      );
      if (!shouldContinue) return;
    }
  }

  private waitForStopAttempt(
    revision: number
  ): Promise<StopAttemptResult> {
    return new Promise<StopAttemptResult>((resolve) => {
      let settled = false;
      let timer: ReturnType<typeof setTimeout>;
      const pendingWait = {
        cancel: () => finish('cancelled'),
      };
      const finish = (result: StopAttemptResult): void => {
        if (settled) return;
        settled = true;
        clearTimeout(timer);
        if (this.pendingStopWait === pendingWait) {
          this.pendingStopWait = undefined;
        }
        resolve(result);
      };

      timer = setTimeout(() => finish('failed'), stopAttemptTimeoutMs);
      this.pendingStopWait = pendingWait;
      try {
        void this.client.stop().then(
          () => finish('succeeded'),
          () => finish('failed')
        );
      } catch {
        finish('failed');
      }
    }).then((result) =>
      revision === this.gateRevision ? result : 'cancelled'
    );
  }

  private async isConnectionAllowed(): Promise<boolean> {
    if (!this.isActive) return false;
    try {
      return !!(await this.sessionProvider.getSession())?.accessToken;
    } catch {
      this.reportLifecycleError('gate-reconciliation-failed');
      return false;
    }
  }

  private waitForStopRetry(
    delay: number,
    revision: number
  ): Promise<boolean> {
    return new Promise((resolve) => {
      let settled = false;
      let timer: ReturnType<typeof setTimeout>;
      const pendingWait = {
        cancel: () => finish(false),
      };
      const finish = (shouldContinue: boolean): void => {
        if (settled) return;
        settled = true;
        clearTimeout(timer);
        if (this.pendingStopWait === pendingWait) {
          this.pendingStopWait = undefined;
        }
        resolve(shouldContinue);
      };

      timer = setTimeout(
        () => finish(revision === this.gateRevision),
        delay
      );
      this.pendingStopWait = pendingWait;
    });
  }

  private cancelPendingStopWait(): void {
    this.pendingStopWait?.cancel();
  }

  private reportLifecycleError(event: SignalRLifecycleErrorEvent): void {
    try {
      this.onLifecycleError?.(event);
    } catch {
      // Ошибка обработчика не должна мешать безопасной очистке.
    }
  }
}

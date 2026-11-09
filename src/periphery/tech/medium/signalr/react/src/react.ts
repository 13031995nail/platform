import {
  createContext,
  createElement,
  useCallback,
  useContext,
  useLayoutEffect,
  useRef,
  useState,
  type ReactNode,
} from 'react';
import {useSessionProvider} from '@tmobis/core/auth/session-context';
import {useMessageDispatcher} from '@tmobis/core/modules/provider';
import type SignalRClient from './client';
import {createSignalRMessageProducer} from './message-producer';
import SignalRService, {type SignalRConnectionOptions} from './service';
import type {SignalRMessageHandler} from './types';

const SignalRContext = createContext<SignalRService | undefined>(undefined);

export interface SignalRProviderProps {
  options?: SignalRConnectionOptions;
  children?: ReactNode;
}

export function SignalRProvider({
  options,
  children,
}: SignalRProviderProps): ReactNode {
  const sessionProvider = useSessionProvider();
  const messageDispatcher = useMessageDispatcher();
  const [service] = useState(() => new SignalRService({
    ...options,
    sessionProvider,
  }));

  useLayoutEffect(() => {
    const unregisterProducer = messageDispatcher.registerProducer(
      createSignalRMessageProducer(service.client)
    );
    void service.initialize().catch((error) => {
      console.error('[signalr] Ошибка инициализации', error);
    });

    return () => {
      unregisterProducer();
      void service.dispose().catch((error) => {
        console.error('[signalr] Ошибка очистки', error);
      });
    };
  }, [messageDispatcher, service]);

  return createElement(
    SignalRContext.Provider,
    {value: service},
    children
  );
}

export function useSignalRService(): SignalRService {
  const service = useContext(SignalRContext);
  if (!service) {
    throw new Error('useSignalRService must be used within a SignalRProvider');
  }
  return service;
}

export function useSignalR(): SignalRClient {
  return useSignalRService().client;
}

export function useSignalRHandler<TPayload>(
  type: string,
  handler: SignalRMessageHandler<TPayload>
): void {
  const service = useSignalRService();
  const subscribe = useCallback(
    (callback: SignalRMessageHandler<TPayload>) => service.on(type, callback),
    [service, type]
  );
  useSignalRSubscription(handler, subscribe);
}

export function useSignalRGlobalHandler(
  handler: SignalRMessageHandler<unknown>
): void {
  const service = useSignalRService();
  const subscribe = useCallback(
    (callback: SignalRMessageHandler<unknown>) => service.onGlobal(callback),
    [service]
  );
  useSignalRSubscription(handler, subscribe);
}

type SignalRSubscriber<TPayload> = (
  handler: SignalRMessageHandler<TPayload>
) => () => void;

function useSignalRSubscription<TPayload>(
  handler: SignalRMessageHandler<TPayload>,
  subscribe: SignalRSubscriber<TPayload>
): void {
  const handlerRef = useRef(handler);

  useLayoutEffect(() => {
    handlerRef.current = handler;
  }, [handler]);

  useLayoutEffect(
    () => subscribe(
      (payload, context) => handlerRef.current(payload, context)
    ),
    [subscribe]
  );
}

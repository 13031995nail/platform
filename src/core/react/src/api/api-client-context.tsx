import React, {
  createContext,
  PropsWithChildren,
  useContext,
  useMemo,
} from 'react';
import type { ApiClient } from './create-api-client';

const ApiClientContext = createContext<ApiClient | null>(null);

export type ApiClientProviderProps = PropsWithChildren<{
  client: ApiClient;
}>;

/**
 * Предоставление HTTP-клиента дереву React.
 */
export function ApiClientProvider({
  client,
  children,
}: ApiClientProviderProps) {
  const value = useMemo(() => client, [client]);

  return (
    <ApiClientContext.Provider value={value}>
      {children}
    </ApiClientContext.Provider>
  );
}

/**
 * Получение HTTP-клиента из контекста.
 */
export function useApiClient(): ApiClient {
  const client = useContext(ApiClientContext);
  if (!client) {
    throw new Error('useApiClient должен использоваться внутри ApiClientProvider.');
  }
  return client;
}

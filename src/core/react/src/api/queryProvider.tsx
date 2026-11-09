import React, { PropsWithChildren, useEffect, useMemo } from 'react';
import { subscribeApiClientCacheInvalidation } from './query/query-scope';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

export default function QueryProvider(props: PropsWithChildren) {
  const queryClient = useMemo(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            refetchOnWindowFocus: false,
          },
        },
      }),
    []
  );

  useEffect(() => subscribeApiClientCacheInvalidation((clientId, previousRevision) => {
    // Отмена сетевых запросов предыдущей сессии до удаления кеша.
    // Новая ревизия не затрагивается даже при параллельном старте новых запросов.
    const queryKey = ['api-client', clientId, previousRevision];
    void queryClient.cancelQueries({ queryKey }).then(() => {
      queryClient.removeQueries({ queryKey });
    });
  }), [queryClient]);

  return (
    <QueryClientProvider client={queryClient}>
      {props.children}
    </QueryClientProvider>
  );
}

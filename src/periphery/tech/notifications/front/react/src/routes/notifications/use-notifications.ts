import { useQuery, useQueryClient } from '@tmobis/core/api/query';
import { createMessageApi } from '@tmobis/core/api/services/message-api';
import { useApiQueryScope } from '@tmobis/core/api/query/use-api-query-scope';
import { shouldRetryQuery } from '@tmobis/core/api/query/query-policy';
import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  GetNotifications,
  getNotificationsQueryKey,
} from '../../messages/get-notifications';
import {
  getUnreadNotificationCountQueryKey,
} from '../../messages/get-unread-notification-count';

export function useNotifications(perPage: number) {
  const [page, setPage] = useState(1);
  const queryClient = useQueryClient();
  const { client, makeQueryKey } = useApiQueryScope();
  const api = useMemo(() => createMessageApi(client), [client]);
  const message = useMemo(
    () => new GetNotifications(page, perPage),
    [page, perPage]
  );
  const notificationsQuery = useQuery({
    queryKey: makeQueryKey(getNotificationsQueryKey(page, perPage)),
    queryFn: ({signal}) => api.sendMessage(message, undefined, signal),
    retry: shouldRetryQuery,
  });

  useEffect(() => {
    if (notificationsQuery.data) {
      void queryClient.invalidateQueries({
        queryKey: makeQueryKey(getUnreadNotificationCountQueryKey()),
      });
    }
  }, [notificationsQuery.data, queryClient, makeQueryKey]);

  const total = notificationsQuery.data?.totalCount ?? 0;
  const pageCount = Math.max(1, Math.ceil(total / perPage));
  const goToPreviousPage = useCallback(() => {
    setPage(current => Math.max(1, current - 1));
  }, []);
  const goToNextPage = useCallback(() => {
    setPage(current => current + 1);
  }, []);

  return {
    goToNextPage,
    goToPreviousPage,
    notificationsQuery,
    page,
    pageCount,
    total,
  };
}

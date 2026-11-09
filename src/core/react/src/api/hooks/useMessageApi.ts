import { useMemo } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import type { QueryKey } from '@tanstack/react-query';
import {
  getMessageMetadata,
  InferMessageResult,
  IMessage,
} from '../../models/message';
import { getMessageQueryKey, createMessageApi } from '../services/message-api';
import { createWebApiMessageProducer } from '../message-producer';
import { useOnApiError, useUpdateApi } from './useBaseApi';
import { shouldRetryQuery } from '../query/query-policy';
import {useOptionalMessageDispatcher} from '../../modules/provider';
import { useApiQueryScope } from '../query/use-api-query-scope';

export interface UseMessageApiOptions {
  queryKey?: QueryKey;
  onNotFound?: () => void;
}

export interface SendMessageMutationOptions<TMessageResult> {
  onSuccess?: (result: TMessageResult) => void;
  onError?: (error: Error) => void;
}

export const useMessageApi = <
  TMessage extends IMessage,
  TMessageResult = InferMessageResult<TMessage>
>(
  message: TMessage,
  options: UseMessageApiOptions = {}
) => {
  const { client, makeQueryKey } = useApiQueryScope();
  const service = useMemo(() => createMessageApi(client), [client]);
  const webProducer = useMemo(() => createWebApiMessageProducer(service), [service]);
  const queryClient = useQueryClient();
  const messageDispatcher = useOptionalMessageDispatcher();
  const metadata = getMessageMetadata(message);
  const isQuery = metadata.type === 'query';
  const dispatchMessage = (signal?: AbortSignal) => messageDispatcher
    ? messageDispatcher.send<TMessage, TMessageResult>(message, webProducer, signal)
    : service.sendMessage<TMessage, TMessageResult>(message, metadata, signal);
  let cacheKey: QueryKey = options.queryKey
    ?? ['message', metadata.type, metadata.group ?? '', metadata.name];

  if (isQuery) {
    cacheKey = options.queryKey
      ?? getMessageQueryKey(message, metadata);
  }

  const request = useUpdateApi<void, TMessageResult>({
    func: () => isQuery
      ? queryClient.fetchQuery({
          queryKey: makeQueryKey(cacheKey),
          queryFn: ({signal}) => dispatchMessage(signal),
          retry: shouldRetryQuery,
        })
      : dispatchMessage(),
  });

  useOnApiError(request.isError, request.error, options.onNotFound);

  const send = (
    mutationOptions?: SendMessageMutationOptions<TMessageResult>
  ) => {
    return request.updateApi(undefined, mutationOptions);
  };

  return {
    data: request.data,
    isLoading: request.isPending,
    isError: request.isError,
    error: request.error,
    sendMessage: send,
  };
};

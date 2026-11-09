import { useMemo } from 'react';
import { createEntityApi, type PagedResult } from '../services/entity-api';
import { useApiQueryScope } from '../query/use-api-query-scope';
import {useQueryApi, useUpdateApi} from './useBaseApi';
import {UpdateEntityRequest} from '../types/UpdateEntityRequest';
import {useQueryClient} from '@tanstack/react-query';
import {ListFilterValues} from "../filters";
import { IEntity } from '../../models/entity';

// Сборка сервиса принадлежит React-слою, сам сервис не зависит от React.
function useEntityService() {
  const { client, makeQueryKey } = useApiQueryScope();
  const service = useMemo(() => createEntityApi(client), [client]);
  return { service, makeQueryKey };
}

export interface GetEntityRequest {
  id?: NonEmptyString;
  viewName?: string;
  entityType: string;
  enabled?: boolean;
  onNotFound?: () => void;
}

export const useGetEntity = <T extends IEntity>({
  id, viewName, entityType, enabled = true, onNotFound
}: GetEntityRequest) => {
  const { service, makeQueryKey } = useEntityService();
  const isQueryEnabled = enabled && id !== undefined;
  const { data, isLoading, isError, error, refetch } = useQueryApi({
    cacheKey: makeQueryKey([entityType, id, viewName]),
    enabled: isQueryEnabled,
    onNotFound: onNotFound,
    func: async (signal): Promise<T> => {
      const response = await service.getEntity<T>(id!, entityType, viewName, signal);
      return response.data;
    },
  });

  return {
    model: data,
    isLoadingModel: isLoading,
    isError,
    error,
    refetch
  };
};

export interface GetEntitiesRequest {
  page?: number;
  perPage?: number;
  sorting?: string;
  viewName?: string;
  entityType: string;
  enabled?: boolean;
  onNotFound?: () => void;
  filter?: ListFilterValues;
  search?: string;
  includeTotalCount?: boolean;
}

export const useGetEntities = <T extends IEntity>({
  page, perPage, sorting,
  viewName, entityType, enabled = true,
  onNotFound, filter, search, includeTotalCount
}: GetEntitiesRequest) => {
  const { service, makeQueryKey } = useEntityService();
  const { data, isLoading, isError, error, refetch } = useQueryApi<PagedResult<T>>({
    cacheKey: makeQueryKey([entityType, 'list', viewName, perPage, page, sorting, filter, search, includeTotalCount]),
    enabled: enabled,
    onNotFound: onNotFound,
    func: (signal): Promise<PagedResult<T>> => service.getEntities<T>({
      entityType, page, perPage, viewName, sorting, filter, search, includeTotalCount
    }, signal),
  });

  return {
    data,
    isLoading,
    isError,
    error,
    refetch,
  };
};

export const useGetEntityCount = ({
  sorting, viewName, entityType, enabled = true, onNotFound, filter, search
}: GetEntitiesRequest) => {
  const { service, makeQueryKey } = useEntityService();
  const { data, isLoading, isError, error, refetch } = useQueryApi({
    cacheKey: makeQueryKey([entityType, 'count', viewName, sorting, filter, search]),
    enabled: enabled,
    onNotFound: onNotFound,
    func: async (signal): Promise<number> => {
      const response = await service.getEntityCount({
        entityType, viewName, sorting, filter, search
      }, signal);
      return response.data.count;
    },
  });

  return {
    count: data,
    isLoading,
    isError,
    error,
    refetch,
  };
};

export const useUpdateEntity = <T extends IEntity>({
  entityType, onSuccess, onError, id
}: UpdateEntityRequest<T> & { id?: string }) => {
  const { service, makeQueryKey } = useEntityService();
  const queryClient = useQueryClient();
  const { updateApi, isPending } = useUpdateApi({
    func: async (data: T) => {
      const response = await service.updateEntity(data, entityType, id);
      return response.data;
    },
    onSuccess: (updatedData: T) => {
      const id = updatedData.id;
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, id]) });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'list']) });
      onSuccess?.(updatedData);
    },
    onError
  });

  return {
    updateEntity: updateApi, isPending
  };
};

export const useDeleteEntity = ({
  entityType, onSuccess, onError
}: UpdateEntityRequest<string>) => {
  const { service, makeQueryKey } = useEntityService();
  const queryClient = useQueryClient();
  const { updateApi: deleteApi, isPending } = useUpdateApi({
    returnResult: false,
    func: async (id: string) => {
      await service.deleteEntity(id, entityType);
      return id;
    },
    onSuccess: (id: string) => {
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, id]) });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'list']) });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'count']) });
      onSuccess?.(id);
    },
    onError
  });

  return {
    deleteEntity: deleteApi, isPending
  };
};

export const useDeleteEntities = ({
  entityType, onSuccess, onError
}: UpdateEntityRequest<string[]>) => {
  const { service, makeQueryKey } = useEntityService();
  const queryClient = useQueryClient();
  const { updateApi: deleteApi, isPending } = useUpdateApi({
    returnResult: false,
    func: async (ids: string[]) => {
      const response = await service.deleteEntities(ids, entityType);
      return response.data;
    },
    onSuccess: (ids: string[]) => {
      ids.forEach((id: string) => {
        queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, id]) });
      });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'list']) });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'count']) });
      onSuccess?.(ids);
    },
    onError
  });

  return {
    deleteEntities: deleteApi, isPending
  };
};

export const useCreateEntity = <T extends IEntity>({
  entityType, onSuccess, onError
}: UpdateEntityRequest<T>) => {
  const { service, makeQueryKey } = useEntityService();
  const queryClient = useQueryClient();
  const { updateApi: createApi, isPending } = useUpdateApi({
    returnResult: false,
    func: async (data: T) => {
      const response = await service.createEntity(data, entityType);
      return response.data;
    },
    onSuccess: (result: T) => {
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'list']) });
      queryClient.invalidateQueries({ queryKey: makeQueryKey([entityType, 'count']) });
      onSuccess?.(result);
    },
    onError
  });

  return {
    createEntity: createApi, isPending
  };
};
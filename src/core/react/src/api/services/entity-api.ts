import type { ApiClient } from '../create-api-client';
import {ListFilterValues, toUrlFilter} from "../filters";

import { IEntity } from '../../models/entity';

export interface GetEndpointParams {
  entityType: string,
  route?: string,
  urlParams?: URLSearchParams
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number | null;
}

const getEndpoint = ({
  entityType, route, urlParams
} : GetEndpointParams) =>
  `/api/${entityType}${route ? '/' + route : ''}${urlParams ? '?' + urlParams?.toString() : ''}`;

export const getEntity = <T extends IEntity>(
id: string,
entityType: string,
viewName: string | undefined,
client: ApiClient,
signal?: AbortSignal) => {
  const urlParams = new URLSearchParams();
  if (viewName) {
    urlParams.append('viewName', viewName);
  }
  const url = getEndpoint({entityType, route: id, urlParams});
  return client.get<T>(url, { signal });
}

export interface GetEntitiesParams {
  entityType: string;
  page?: number;
  perPage?: number;
  viewName?: string;
  sorting?: string;
  filter?: ListFilterValues;
  search?: string;
  includeTotalCount?: boolean;
}

export const getEntities = async <T extends IEntity>({
  entityType, page, perPage, viewName, sorting, filter, search, includeTotalCount
}: GetEntitiesParams, client: ApiClient, signal?: AbortSignal): Promise<PagedResult<T>> => {
  const urlParams = new URLSearchParams();
  if (page) {
    urlParams.append('page', page.toString());
  }
  if (perPage) {
    urlParams.append('perPage', perPage.toString());
  }
  if (viewName) {
    urlParams.append('viewName', viewName);
  }
  if (sorting) {
    urlParams.append('sorting', sorting);
  }
  if (filter) {
    toUrlFilter(filter).forEach((filter: string) => {
      urlParams.append('filter', filter);
    });
  }
  if (search) {
    urlParams.append('search', search);
  }
  if (includeTotalCount !== undefined) {
    urlParams.append('includeTotalCount', includeTotalCount.toString());
  }

  const url = getEndpoint({entityType, urlParams});
  const result = await client.get<PagedResult<T>>(url, { signal });
  return result.data;
}

export const getEntityCount = ({
  entityType, viewName, sorting, filter, search
}: GetEntitiesParams, client: ApiClient, signal?: AbortSignal) => {
  const urlParams = new URLSearchParams();
  if (viewName) {
    urlParams.append('viewName', viewName);
  }
  if (sorting) {
    urlParams.append('sorting', sorting);
  }
  if (search) {
    urlParams.append('search', search);
  }
  if (filter) {
    toUrlFilter(filter).forEach((filter: string) => {
      urlParams.append('filter', filter);
    });
  }
  const url = getEndpoint({entityType, route: 'count', urlParams});
  return client.get<{ count: number }>(url, { signal });
}

export const updateEntity = <T extends IEntity>(
data: T,
entityType: string,
client: ApiClient,
id: string | undefined = data.id,
) => {
  if (!id?.trim()) {
    throw new Error('Не указан идентификатор обновляемой сущности');
  }
  const url = getEndpoint({entityType, route: id});
  return client.put<T>(url, data);
}

export const createEntity = <T extends IEntity>(
data: T,
entityType: string,
client: ApiClient,
) => {
  const url = getEndpoint({entityType});
  return client.post<T>(url, data);
}

export const deleteEntity = (
id: string,
entityType: string,
client: ApiClient,
) => {
  const url = getEndpoint({entityType, route: id});
  return client.delete(url);
}

export const deleteEntities = (
ids: string[],
entityType: string,
client: ApiClient,
) => {
  const url = getEndpoint({entityType, route: 'deleteByIds'});
  return client.post(url, ids);
}
/** Низкоуровневые операции, созданные для конкретного HTTP-клиента. */
export function createEntityApi(client: ApiClient) {
  return {
    getEntity: <T extends IEntity>(id: string, type: string, viewName?: string, signal?: AbortSignal) => getEntity<T>(id, type, viewName, client, signal),
    getEntities: <T extends IEntity>(params: GetEntitiesParams, signal?: AbortSignal) => getEntities<T>(params, client, signal),
    getEntityCount: (params: GetEntitiesParams, signal?: AbortSignal) => getEntityCount(params, client, signal),
    updateEntity: <T extends IEntity>(entity: T, type: string, id?: string) => updateEntity(entity, type, client, id),
    createEntity: <T extends IEntity>(entity: T, type: string) => createEntity(entity, type, client),
    deleteEntity: (id: string, type: string) => deleteEntity(id, type, client),
    deleteEntities: (ids: string[], type: string) => deleteEntities(ids, type, client),
  };
}
export type EntityApi = ReturnType<typeof createEntityApi>;

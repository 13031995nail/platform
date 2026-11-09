import axios, {
  AxiosError,
  AxiosHeaders,
  AxiosInstance,
  CreateAxiosDefaults,
} from 'axios';
import { ApiError } from './api-error';

export type ApiClient = AxiosInstance;

export type GetAccessToken = () => Promise<string | undefined>;

export type CreateApiClientConfig = CreateAxiosDefaults & {
  getAccessToken?: GetAccessToken;
};

const accessTokenGetters = new WeakMap<ApiClient, GetAccessToken>();

const CORRELATION_HEADERS = [
  'x-correlation-id',
  'x-request-id',
  'correlation-id',
] as const;

/**
 * Регистрация получения access-токена для конкретного клиента.
 */
export function configureApiAuth(
  client: ApiClient,
  getAccessToken: GetAccessToken
): void {
  accessTokenGetters.set(client, getAccessToken);
}

/**
 * Создание HTTP-клиента с единым маппингом ошибок в ApiError.
 */
export function createApiClient(config: CreateApiClientConfig = {}): ApiClient {
  const { getAccessToken, ...axiosConfig } = config;

  const client = axios.create({
    withCredentials: false,
    headers: {
      'Content-Type': 'application/json',
    },
    ...axiosConfig,
  });

  if (getAccessToken) {
    accessTokenGetters.set(client, getAccessToken);
  }

  client.interceptors.request.use(async (request) => {
    const getter = accessTokenGetters.get(client);
    const accessToken = await getter?.();
    if (accessToken) {
      const headers = AxiosHeaders.from(request.headers);
      headers.set('Authorization', `Bearer ${accessToken}`);
      request.headers = headers;
    }
    return request;
  });

  client.interceptors.response.use(
    (response) => response,
    (error: unknown) => Promise.reject(toApiError(error))
  );

  return client;
}

function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) {
    return error;
  }

  if (!axios.isAxiosError(error)) {
    return new ApiError({
      message: error instanceof Error ? error.message : 'Неизвестная ошибка',
      code: 'UNKNOWN',
      original: error,
    });
  }

  const axiosError = error as AxiosError;
  const response = axiosError.response;

  if (response) {
    const details = response.data;
    return new ApiError({
      message: resolveHttpMessage(details, response.status),
      status: response.status,
      code: resolveHttpCode(details, response.status),
      details,
      correlationId: readCorrelationId(response.headers),
      original: axiosError,
    });
  }

  return new ApiError({
    message: resolveNetworkMessage(axiosError),
    code: resolveNetworkCode(axiosError),
    original: axiosError,
  });
}

function resolveHttpMessage(data: unknown, status: number): string {
  if (data && typeof data === 'object') {
    const body = data as Record<string, unknown>;
    if (typeof body.message === 'string' && body.message) {
      return body.message;
    }
    if (typeof body.title === 'string' && body.title) {
      return body.title;
    }
    if (typeof body.error === 'string' && body.error) {
      return body.error;
    }
  }

  if (typeof data === 'string' && data) {
    return data;
  }

  switch (status) {
    case 400:
      return 'Bad Request';
    case 401:
      return 'Unauthorized';
    case 403:
      return 'Forbidden';
    case 404:
      return 'Not Found';
    case 500:
      return 'Internal Server Error';
    default:
      return 'An error occurred';
  }
}

function resolveHttpCode(data: unknown, status: number): string {
  if (data && typeof data === 'object') {
    const body = data as Record<string, unknown>;
    if (typeof body.code === 'string' && body.code) {
      return body.code;
    }
  }
  return `HTTP_${status}`;
}

function resolveNetworkCode(error: AxiosError): string {
  if (error.code === 'ECONNABORTED' || error.code === 'ETIMEDOUT') {
    return 'TIMEOUT';
  }
  if (error.code === 'ERR_CANCELED') {
    return 'CANCELED';
  }
  return error.code || 'NETWORK_ERROR';
}

function resolveNetworkMessage(error: AxiosError): string {
  const code = resolveNetworkCode(error);
  if (code === 'TIMEOUT') {
    return 'Превышено время ожидания ответа сервера';
  }
  if (code === 'CANCELED') {
    return 'Запрос отменён';
  }
  return error.message || 'Ошибка сети';
}

function readCorrelationId(headers: unknown): string | undefined {
  if (!headers || typeof headers !== 'object') {
    return undefined;
  }

  const withGet = headers as { get?: (name: string) => unknown };
  if (typeof withGet.get === 'function') {
    for (const name of CORRELATION_HEADERS) {
      const value = withGet.get(name);
      if (typeof value === 'string' && value) {
        return value;
      }
    }
  }

  const record = headers as Record<string, unknown>;
  for (const name of CORRELATION_HEADERS) {
    const value = record[name] ?? record[name.toLowerCase()];
    if (typeof value === 'string' && value) {
      return value;
    }
  }

  return undefined;
}

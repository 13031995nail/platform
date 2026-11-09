import { isApiError } from '../api-error';

const RETRYABLE_HTTP_STATUSES = new Set([408, 429, 502, 503, 504]);
const RETRYABLE_NETWORK_CODES = new Set([
  'TIMEOUT',
  'NETWORK_ERROR',
  'ERR_NETWORK',
  'ECONNRESET',
  'EAI_AGAIN',
]);

/** Only safe, transient failures are retried. Cancellation and programming errors are terminal. */
export function shouldRetryQuery(failureCount: number, error: unknown): boolean {
  if (failureCount >= 3 || !isApiError(error)) {
    return false;
  }
  return error.status != null
    ? RETRYABLE_HTTP_STATUSES.has(error.status)
    : RETRYABLE_NETWORK_CODES.has(error.code ?? '');
}

/** 404 is an expected API result: the consumer handles it via isError/isNotFound. */
export function shouldThrowOnError(error: unknown): boolean {
  return !(isApiError(error) && error.status === 404);
}

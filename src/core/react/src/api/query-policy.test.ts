import assert from 'node:assert/strict';
import { test } from 'node:test';
import { ApiError } from './api-error';
import { shouldRetryQuery, shouldThrowOnError } from './query/query-policy';

test('retry only for transient HTTP errors, at most three times', () => {
  for (const status of [408, 429, 502, 503, 504]) {
    const error = new ApiError({ message: 'Transient', status });
    assert.equal(shouldRetryQuery(0, error), true);
    assert.equal(shouldRetryQuery(2, error), true);
    assert.equal(shouldRetryQuery(3, error), false);
  }
  for (const status of [400, 401, 403, 404, 409, 422, 500]) {
    assert.equal(shouldRetryQuery(0, new ApiError({ message: 'Permanent', status })), false);
  }
});

test('network timeout is retryable but canceled and unknown errors are not', () => {
  for (const code of ['TIMEOUT', 'NETWORK_ERROR', 'ERR_NETWORK', 'ECONNRESET']) {
    assert.equal(shouldRetryQuery(0, new ApiError({ message: code, code })), true);
  }
  for (const code of ['CANCELED', 'UNKNOWN', 'ERR_BAD_REQUEST']) {
    assert.equal(shouldRetryQuery(0, new ApiError({ message: code, code })), false);
  }
  assert.equal(shouldRetryQuery(0, new Error('Programming error')), false);
});

test('404 remains visible in query state without going to ErrorBoundary', () => {
  assert.equal(shouldThrowOnError(new ApiError({ message: 'Not found', status: 404 })), false);
  assert.equal(shouldThrowOnError(new ApiError({ message: 'Unavailable', status: 503 })), true);
});

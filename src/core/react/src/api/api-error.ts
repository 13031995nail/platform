/**
 * Единая модель ошибки HTTP/API для web и React Native.
 */
export class ApiError extends Error {
  readonly status?: number;
  readonly code?: string;
  readonly details?: unknown;
  readonly correlationId?: string;
  readonly original?: unknown;

  constructor(options: {
    message: string;
    status?: number;
    code?: string;
    details?: unknown;
    correlationId?: string;
    original?: unknown;
  }) {
    super(options.message);
    this.name = 'ApiError';
    this.status = options.status;
    this.code = options.code;
    this.details = options.details;
    this.correlationId = options.correlationId;
    this.original = options.original;

    Object.setPrototypeOf(this, new.target.prototype);
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

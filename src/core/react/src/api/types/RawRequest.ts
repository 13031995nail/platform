import type { QueryKey } from "@tanstack/react-query";
import type { ApiError } from "../api-error";

export interface RawQuery<T> {
  cacheKey: QueryKey;
  enabled?: boolean;
  onNotFound?: () => void;
  func: (signal: AbortSignal) => Promise<T>;
}

export interface RawUpdate<TData, TResult = TData> {
  onSuccess?: (result: TResult) => void,
  onError?: (e: Error | ApiError) => void,
  func: (data: TData) => Promise<TResult>,
  returnResult?: boolean,
}

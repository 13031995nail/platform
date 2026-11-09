import type { ApiError } from "../api-error";

export interface UpdateEntityRequest<T> {
  entityType: string;
  onSuccess?: (updatedData: T) => void,
  onError?: (e: Error | ApiError) => void,
}

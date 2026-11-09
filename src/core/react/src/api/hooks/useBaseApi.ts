import { useEffect } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { isApiError } from '../api-error';
import { shouldRetryQuery, shouldThrowOnError } from '../query/query-policy';
import { RawQuery, RawUpdate } from '../types/RawRequest';

export const useOnApiError = (
    isError?: boolean,
    error?: Error | null,
    onNotFound?: () => void) => {
    useEffect(() => {
        if (isError && isApiError(error) && error.status === 404) {
            if (typeof onNotFound === 'function') {
                onNotFound();
            }
        }
    }, [error, isError, onNotFound]);

    return {
        error, isError, onNotFound
    };
};

export const useQueryApi = <T>({
    func, cacheKey, enabled = true, onNotFound
}: RawQuery<T>) => {
    const { data, isLoading, isError, error, refetch } = useQuery({
        queryKey: cacheKey,
        retry: shouldRetryQuery,
        enabled: enabled,
        queryFn: ({signal}) => func(signal),
        throwOnError: shouldThrowOnError,
    });

    useOnApiError(isError, error, onNotFound);

    return {
        data,
        isLoading,
        isError,
        error,
        isNotFound: isApiError(error) && error.status === 404,
        refetch
    };
};

export const useUpdateApi = <TData, TResult = TData>({
    func, onSuccess, onError, returnResult = true,
}: RawUpdate<TData, TResult>) => {
    const {
        mutateAsync: updateApi,
        isPending,
        data,
        isError,
        error,
    } = useMutation({
        mutationFn: func,
        onSuccess: (response, variables) => onSuccess?.(
            returnResult ? response : variables as unknown as TResult
        ),
        onError: (e) => onError?.(e),
    });

    return { updateApi, isPending, data, isError, error };
};

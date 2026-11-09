import { FieldErrors, FieldValues } from 'react-hook-form';
import { isApiError } from './api-error';

export const formatError = (error: unknown) => {
  if (isApiError(error)) {
    return error.details ?? error.message ?? 'Ошибка сервера';
  }

  if (error instanceof Error) {
    return error.message || 'Неизвестная ошибка';
  }

  return 'Неизвестная ошибка';
};

export const getErrorMessages = <T extends FieldValues>(
  errors: FieldErrors<T> | FieldErrors<T>[] | undefined
): string[] => {
  if (!errors) {
    return [];
  }

  return Object.values(errors).reduce<string[]>((acc, error) => {
    if (!error) {
      return acc;
    }

    // @ts-ignore
    if (error.message) {
      // @ts-ignore
      return [...acc, error.message as string];
    }

    if (typeof error === 'object') {
      return [...acc, ...getErrorMessages(error as FieldErrors<FieldValues>)];
    }

    return acc;
  }, []);
};

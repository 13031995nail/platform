import {useEffect} from 'react';
import type {FormStateController} from './use-form-state';

/**
 * Синхронизация состояния формы с внешним флагом загрузки.
 */
export function useFormStateLoading(
  formState: Pick<FormStateController, 'beginLoading' | 'endLoading'>,
  isLoading: boolean
) {
  useEffect(() => {
    if (isLoading) {
      formState.beginLoading();
      return () => {
        formState.endLoading();
      };
    }
  }, [formState.beginLoading, formState.endLoading, isLoading]);
}

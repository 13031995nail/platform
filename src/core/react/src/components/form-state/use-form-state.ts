import {useCallback, useMemo, useRef, useState} from 'react';
import {FormState, getFormStateLabel} from './form-state';

export interface FormStateController {
  /** Текущее состояние формы */
  state: FormState;
  /** Форма готова к работе */
  isReady: boolean;
  /** Идёт загрузка данных */
  isLoading: boolean;
  /** Идёт сохранение */
  isSaving: boolean;
  /** Текст текущего состояния */
  label: string;
  /** Явная установка состояния */
  setState: (state: FormState) => void;
  /** Начало асинхронной загрузки */
  beginLoading: () => void;
  /** Завершение асинхронной загрузки */
  endLoading: () => void;
  /** Начало сохранения */
  beginSaving: () => void;
  /** Завершение сохранения */
  endSaving: () => void;
}

function resolveFormState(
  loadingCount: number,
  savingCount: number,
  manualState: FormState | null
): FormState {
  if (manualState !== null) {
    return manualState;
  }

  if (savingCount > 0) {
    return FormState.Saving;
  }

  if (loadingCount > 0) {
    return FormState.Loading;
  }

  return FormState.Ready;
}

/**
 * Хук управления состоянием формы.
 * Поддерживает вложенные асинхронные операции загрузки и сохранения.
 */
export function useFormState(initialState: FormState = FormState.Ready): FormStateController {
  const loadingCountRef = useRef(0);
  const savingCountRef = useRef(0);
  const manualStateRef = useRef<FormState | null>(
    initialState === FormState.Ready ? null : initialState
  );
  const [state, setStateValue] = useState<FormState>(initialState);

  const syncState = useCallback(() => {
    setStateValue(resolveFormState(
      loadingCountRef.current,
      savingCountRef.current,
      manualStateRef.current
    ));
  }, []);

  const setState = useCallback((nextState: FormState) => {
    if (nextState === FormState.Ready) {
      manualStateRef.current = null;
      loadingCountRef.current = 0;
      savingCountRef.current = 0;
    } else {
      manualStateRef.current = nextState;
    }

    syncState();
  }, [syncState]);

  const beginLoading = useCallback(() => {
    manualStateRef.current = null;
    loadingCountRef.current += 1;
    syncState();
  }, [syncState]);

  const endLoading = useCallback(() => {
    loadingCountRef.current = Math.max(0, loadingCountRef.current - 1);
    syncState();
  }, [syncState]);

  const beginSaving = useCallback(() => {
    manualStateRef.current = null;
    savingCountRef.current += 1;
    syncState();
  }, [syncState]);

  const endSaving = useCallback(() => {
    savingCountRef.current = Math.max(0, savingCountRef.current - 1);
    syncState();
  }, [syncState]);

  return useMemo(() => ({
    state,
    isReady: state === FormState.Ready,
    isLoading: state === FormState.Loading,
    isSaving: state === FormState.Saving,
    label: getFormStateLabel(state),
    setState,
    beginLoading,
    endLoading,
    beginSaving,
    endSaving,
  }), [
    beginLoading,
    beginSaving,
    endLoading,
    endSaving,
    setState,
    state,
  ]);
}

/**
 * Состояние формы.
 */
export enum FormState {
  /** Форма готова к работе */
  Ready = 'ready',
  /** Ожидание асинхронной загрузки данных */
  Loading = 'loading',
  /** Ожидание сохранения */
  Saving = 'saving',
}

const formStateLabels: Record<FormState, string> = {
  [FormState.Ready]: '',
  [FormState.Loading]: 'Загрузка...',
  [FormState.Saving]: 'Сохранение...',
};

/**
 * Текст состояния формы для отображения в UI.
 */
export function getFormStateLabel(state: FormState): string {
  return formStateLabels[state];
}

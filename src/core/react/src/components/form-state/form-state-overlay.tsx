import { ReactNode } from 'react';
import { View } from '../ui/view';
import { Spinner } from '../ui/spinner';
import { Text } from '../ui/text';
import { VStack } from '../ui/vstack';
import type { FormStateController } from './use-form-state';
import { TmobisSpinner } from '@tmobis/core/components/spinner';

export interface FormStateOverlayProps {
  formState: Pick<FormStateController, 'isReady' | 'label'>;
  children: ReactNode;
}

/**
 * Универсальное отображение спиннера с текстом состояния, пока форма не готова.
 */
export default function FormStateOverlay({
  formState,
  children,
}: FormStateOverlayProps) {
  if (formState.isReady) {
    return children;
  }

  return (
    <TmobisSpinner label={formState.label} />
  );
}

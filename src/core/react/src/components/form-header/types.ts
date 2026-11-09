import type { ReactNode } from 'react';

export interface FormHeaderProps {
  title?: string;
  isDirty: boolean;
  isSubmitting: boolean;
  submit: (closeAfterSave?: boolean) => Promise<boolean>;
  back: () => void;
  discardAndBack: () => void;
  saveError?: string;
  backLabel?: string;
  saveLabel?: string;
  children?: ReactNode;
}

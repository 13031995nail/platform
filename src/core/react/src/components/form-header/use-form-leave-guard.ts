import { useRef, useState } from 'react';

export type FormLeaveGuardParams = {
  isDirty: boolean;
  isSubmitting: boolean;
  submit: (closeAfterSave?: boolean) => Promise<boolean>;
  back: () => void;
  discardAndBack: () => void;
  saveError?: string;
};

/**
 * Подтверждение выхода с несохраненной формы.
 */
export function useFormLeaveGuard({
  isDirty,
  isSubmitting,
  submit,
  back,
  discardAndBack,
  saveError,
}: FormLeaveGuardParams) {
  const [isConfirmationOpened, setIsConfirmationOpened] = useState(false);
  const [isSaveErrorVisible, setIsSaveErrorVisible] = useState(false);
  const [allowLeave, setAllowLeave] = useState(false);
  const pendingLeave = useRef<(() => void) | null>(null);

  const handleBack = () => {
    if (isDirty) {
      setIsSaveErrorVisible(false);
      setIsConfirmationOpened(true);
      pendingLeave.current = back;
      return;
    }
    back();
  };

  const handleClose = () => {
    setIsConfirmationOpened(false);
    setIsSaveErrorVisible(false);
    pendingLeave.current = null;
  };

  const handleDeny = () => {
    setAllowLeave(true);
    setIsConfirmationOpened(false);
    pendingLeave.current = null;
    discardAndBack();
  };

  const handleApprove = async () => {
    setAllowLeave(true);
    const saved = await submit(true);
    if (!saved) {
      setAllowLeave(false);
      setIsSaveErrorVisible(true);
      return;
    }
    setIsConfirmationOpened(false);
    pendingLeave.current = null;
  };

  const requestLeave = (leave: () => void) => {
    if (!isDirty || allowLeave) {
      leave();
      return;
    }
    pendingLeave.current = leave;
    setIsSaveErrorVisible(false);
    setIsConfirmationOpened(true);
  };

  const alertMessage = isSaveErrorVisible
    ? saveError ?? 'Не удалось сохранить изменения'
    : 'Есть несохраненные изменения. Сохранить изменения?';

  return {
    allowLeave,
    isConfirmationOpened,
    isSaveErrorVisible,
    isSubmitting,
    alertMessage,
    handleBack,
    handleClose,
    handleDeny,
    handleApprove,
    requestLeave,
  };
}

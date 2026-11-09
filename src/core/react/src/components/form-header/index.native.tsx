import { TmobisAlert } from '@tmobis/core/components/alert';
import { TmobisButton } from '@tmobis/core/components/button';
import { Box } from '../ui/box';
import { SafeAreaView } from '../ui/safe-area-view';

import type { FormHeaderProps } from './types';
import { useFormLeaveGuard } from './use-form-leave-guard';

/**
 * Нижняя панель сохранения нативной формы. Заголовок уже в шапке приложения.
 */
export default function FormHeader({
  isDirty,
  isSubmitting,
  submit,
  back,
  discardAndBack,
  saveError,
  saveLabel = 'Сохранить',
}: FormHeaderProps) {
  const guard = useFormLeaveGuard({
    isDirty,
    isSubmitting,
    submit,
    back,
    discardAndBack,
    saveError,
  });

  return (
    <>
      <SafeAreaView edges={['bottom']} className="border-t border-border bg-background">
      <Box
        className="w-full px-4 pt-3 pb-3"
        style={{ elevation: 8 }}
      >
        <TmobisButton
          label={saveLabel}
          size="lg"
          onPress={() => submit()}
          isDisabled={isSubmitting}
          className="w-full"
        />
      </Box>
      </SafeAreaView>

      <TmobisAlert
        isOpen={guard.isConfirmationOpened}
        message={guard.alertMessage}
        isLoading={isSubmitting}
        onClose={guard.handleClose}
        isApproveVisible={!guard.isSaveErrorVisible}
        onDeny={guard.handleDeny}
        isDenyVisible={!guard.isSaveErrorVisible}
        onApprove={guard.handleApprove}
      />
    </>
  );
}

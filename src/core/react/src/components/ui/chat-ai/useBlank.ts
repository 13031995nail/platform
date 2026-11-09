import { useAnimatedReaction } from 'react-native-reanimated';
import { useBlankContext } from './blank-context';
import { useMessageHeight } from './useMessageHeight';

interface UseMessageBlankSizeOptions {
  disabled?: boolean;
  role: 'user' | 'assistant';
}

export function useBlankSize({
  disabled = false,
  role,
}: UseMessageBlankSizeOptions) {
  const context = useBlankContext();
  const targetHeight = role === 'user'
    ? context.userMessageHeight
    : context.assistantMessageHeight;
  const { ref, onLayout } = useMessageHeight(disabled ? undefined : targetHeight);

  useAnimatedReaction(
    () => ({
      user: context.userMessageHeight.value,
      assistant: context.assistantMessageHeight.value,
      container: context.messagesContainerHeight.value,
      disabled,
    }),

    ({ user, assistant, container, disabled: isDisabled }) => {
      'worklet';
      if (isDisabled) return;
      const pairedHeight = role === 'user' ? user : assistant;
      const nextBlank = Math.max(
        0,
        container - pairedHeight - 46
      );
      context.blankSize.value = nextBlank;
    }
  );

  return { ref, onLayout };
}

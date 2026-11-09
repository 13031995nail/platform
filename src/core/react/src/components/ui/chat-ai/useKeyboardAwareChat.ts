import {
  useAnimatedStyle,
  type SharedValue,
} from 'react-native-reanimated';
import { useReanimatedKeyboardAnimation } from 'react-native-keyboard-controller';

export const useKeyboardAwareChat = (blankSize: SharedValue<number>) => {
  const { height } = useReanimatedKeyboardAnimation();
  const listContentStyle = useAnimatedStyle(() => ({
    paddingBottom: blankSize.value + Math.abs(height.value),
  }), [blankSize, height]);

  return { listContentStyle };
};

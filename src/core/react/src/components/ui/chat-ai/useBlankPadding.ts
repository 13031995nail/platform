import { useState } from 'react';
import { runOnJS, useAnimatedReaction, type SharedValue } from 'react-native-reanimated';

/** Передаёт изменения свободного места в обычный web-список через состояние React. */
export function useBlankPadding(blankSize: SharedValue<number>) {
  const [paddingBottom, setPaddingBottom] = useState(0);
  useAnimatedReaction(
    () => blankSize.value,
    (value, previous) => {
      if (value !== previous) {
        runOnJS(setPaddingBottom)(value);
      }
    },
    [blankSize]
  );
  return paddingBottom;
}

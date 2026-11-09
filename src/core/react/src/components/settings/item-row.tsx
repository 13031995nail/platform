import type { ReactNode } from 'react';
import { TmobisButton } from '@tmobis/core/components/button';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { Text } from '@tmobis/core/components/ui/text';

export type SettingsItemRowProps = {
  caption: string;
  description?: string;
  onPress?: () => void;
  control?: ReactNode;
};

/**
 * Строка списка настроек: кнопка или кастомный контрол + текстовое описание.
 */
export function SettingsItemRow({
  caption,
  description,
  onPress,
  control,
}: SettingsItemRowProps) {
  const controlNode = control ?? (
    <TmobisButton
      className="shrink-0"
      label={caption}
      onPress={onPress}
    />
  );

  return (
    <HStack className="items-center gap-3 py-1">
      {controlNode}
      {description ? (
        <Text className="flex-1 text-sm text-foreground-muted">
          {description}
        </Text>
      ) : null}
    </HStack>
  );
}

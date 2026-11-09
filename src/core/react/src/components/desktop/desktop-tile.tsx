import React from 'react';
import { Pressable } from '@tmobis/core/components/ui/pressable';
import { Box } from '@tmobis/core/components/ui/box';
import { Text } from '@tmobis/core/components/ui/text';
import type { DesktopItem } from '@tmobis/core/desktop/types';

export type DesktopTileProps = {
  item: DesktopItem;
  iconSize: number;
  accentColor: string;
  accentBackground: string;
  onPress: () => void;
};

/**
 * Плитка рабочего стола: thumbnail + подпись.
 */
export function DesktopTile({
  item,
  iconSize,
  accentColor,
  accentBackground,
  onPress,
}: DesktopTileProps) {
  const color = item.color ?? accentColor;
  const thumbnailSize = Math.round(iconSize * 1.5);

  return (
    <Pressable
      onPress={onPress}
      accessibilityRole="button"
      accessibilityLabel={item.title ?? item.caption}
      className="items-center p-4"
    >
      <Box
        className="items-center justify-center rounded-xl mb-3"
        style={{
          width: thumbnailSize,
          height: thumbnailSize,
          backgroundColor: accentBackground,
        }}
      >
        {item.icon({ color, size: iconSize })}
      </Box>
      <Text
        className="text-center text-base text-typography-900"
        numberOfLines={2}
      >
        {item.caption}
      </Text>
    </Pressable>
  );
}

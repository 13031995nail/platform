import React, { useMemo } from 'react';
import { useWindowDimensions } from 'react-native';
import { useRouter } from 'expo-router';
import { Box } from '@tmobis/core/components/ui/box';
import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import type { DesktopItem, DesktopLayoutConfig } from '@tmobis/core/desktop/types';
import { DesktopTile } from './desktop-tile';

export type DesktopGridProps = {
  items: DesktopItem[];
  layout: DesktopLayoutConfig;
};

/**
 * Добавление альфы к hex-цвету (#rrggbb → #rrggbbaa).
 */
function withAlpha(hex: string, alphaHex: string): string {
  if (/^#[0-9a-fA-F]{6}$/.test(hex)) {
    return `${hex}${alphaHex}`;
  }
  return hex;
}

/**
 * Сетка иконок-кнопок рабочего стола.
 */
export function DesktopGrid({ items, layout }: DesktopGridProps) {
  const router = useRouter();
  const colors = useGluestackColors();
  const { width } = useWindowDimensions();

  const accentColor = colors.primary ?? '#F05B25';
  const accentBackground = withAlpha(accentColor, '26'); // ~15% opacity

  const columns = Math.max(1, layout.columns);
  const cellWidth = useMemo(() => {
    const horizontalPadding = 32;
    const totalGap = layout.gap * (columns - 1);
    return Math.floor((width - horizontalPadding - totalGap) / columns);
  }, [width, columns, layout.gap]);

  return (
    <Box
      className="flex-1 flex-row flex-wrap p-4"
      style={{ gap: layout.gap }}
    >
      {items.map((item) => (
        <Box key={item.id} style={{ width: cellWidth }}>
          <DesktopTile
            item={item}
            iconSize={layout.iconSize}
            accentColor={accentColor}
            accentBackground={accentBackground}
            onPress={() => router.push(item.href as never)}
          />
        </Box>
      ))}
    </Box>
  );
}

import { TmobisButton } from '@tmobis/core/components/button';
import { Box } from '@tmobis/core/components/ui/box';
import { useWindowDimensions } from 'react-native';
import type { SettingsItem } from '@tmobis/core/settings/types';

export type SettingsActionGridProps = {
  items: SettingsItem[];
  onItemPress: (item: SettingsItem) => void;
  columns?: number;
};

/**
 * Грид акцентных кнопок настроек.
 */
export function SettingsActionGrid({
  items,
  onItemPress,
  columns = 2,
}: SettingsActionGridProps) {
  const { width } = useWindowDimensions();
  const columnCount = width < 640 ? 1 : Math.max(1, columns);

  return (
    <Box
      className="flex-row flex-wrap"
      style={{ gap: 12 }}
    >
      {items.map((item) => (
        <Box
          key={item.id}
          style={{
            flexGrow: 1,
            flexBasis: columnCount === 1 ? '100%' : '48%',
            minWidth: columnCount === 1 ? '100%' : 220,
          }}
        >
          <TmobisButton
            className="w-full"
            label={item.caption}
            onPress={() => onItemPress(item)}
          />
        </Box>
      ))}
    </Box>
  );
}

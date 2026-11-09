import { useFocusEffect } from '@react-navigation/native';
import { useHeader } from '@tmobis/core/components/header/context';
import { Box } from '@tmobis/core/components/ui/box';
import { Text } from '@tmobis/core/components/ui/text';
import { DesktopGrid } from '@tmobis/core/components/desktop';
import { useModules } from '@tmobis/core/modules/provider';
import { useCallback, useMemo } from 'react';
import { ScrollView } from 'react-native';

/**
 * Экран «Рабочий стол» — сетка иконок-кнопок из модулей / override приложения.
 */
const DesktopScreen = () => {
  const { setHeaderData } = useHeader();
  const { compositionRoot } = useModules();

  const items = useMemo(
    () => compositionRoot.getDesktopItems(),
    [compositionRoot]
  );
  const layout = useMemo(
    () => compositionRoot.getDesktopLayout(),
    [compositionRoot]
  );

  useFocusEffect(
    useCallback(() => {
      setHeaderData({ title: 'Рабочий стол' });
    }, [setHeaderData])
  );

  return (
    <ScrollView style={{ flex: 1 }}>
      <Box className="flex-1 bg-background-0">
        {items.length === 0 ? (
          <Box className="flex-1 items-center justify-center p-8">
            <Text className="text-typography-500 text-center">
              Нет доступных разделов на рабочем столе
            </Text>
          </Box>
        ) : (
          <DesktopGrid items={items} layout={layout} />
        )}
      </Box>
    </ScrollView>
  );
};

export default DesktopScreen;

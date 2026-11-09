import { useMemo } from 'react';
import { ScrollView } from 'react-native';
import { useRouter } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { Box } from '@tmobis/core/components/ui/box';
import { Text } from '@tmobis/core/components/ui/text';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { useModules } from '@tmobis/core/modules/provider';
import { collectSettingsSections } from '@tmobis/core/settings/group-items';
import {
  CORE_SETTINGS_GROUP,
  type SettingsItem,
} from '@tmobis/core/settings/types';
import { SettingsActionGrid } from './action-grid';
import { SettingsItemRow } from './item-row';

function SectionHeading({ label }: { label: string }) {
  const { t } = useTranslation();
  const text = label === CORE_SETTINGS_GROUP
    ? t('settings.group.general', { defaultValue: label })
    : label;

  return (
    <Text className="text-xs font-semibold uppercase text-foreground-muted">
      {text}
    </Text>
  );
}

function SettingsRowItem({
  item,
  onPress,
}: {
  item: SettingsItem;
  onPress: (item: SettingsItem) => void;
}) {
  if (item.component) {
    const Component = item.component;
    return <Component item={item} />;
  }

  return (
    <SettingsItemRow
      caption={item.caption}
      description={item.description}
      onPress={() => onPress(item)}
    />
  );
}

/**
 * Экран настроек: грид акцентных кнопок, затем список кнопка + описание.
 */
export function SettingsView() {
  const { compositionRoot } = useModules();
  const router = useRouter();

  const items = useMemo(
    () => compositionRoot.getSettingsItems(),
    [compositionRoot],
  );

  const actionSections = useMemo(
    () => collectSettingsSections(items, 'action'),
    [items],
  );
  const rowSections = useMemo(
    () => collectSettingsSections(items, 'row'),
    [items],
  );

  const handlePress = (item: SettingsItem) => {
    if (item.onPress) {
      void item.onPress();
      return;
    }
    if (item.href) {
      router.push(item.href as never);
    }
  };

  return (
    <ScrollView
      style={{ flex: 1 }}
      className="bg-background"
    >
      <Box className="flex-1 bg-background p-4">
        <VStack className="gap-6">
          {actionSections.map((section) => (
            <VStack key={`action-${section.group}`} className="gap-3">
              <SectionHeading label={section.group} />
              <SettingsActionGrid
                items={section.items}
                onItemPress={handlePress}
              />
            </VStack>
          ))}

          {rowSections.map((section) => (
            <VStack key={`row-${section.group}`} className="gap-3">
              <SectionHeading label={section.group} />
              <VStack className="gap-3">
                {section.items.map((item) => (
                  <SettingsRowItem
                    key={item.id}
                    item={item}
                    onPress={handlePress}
                  />
                ))}
              </VStack>
            </VStack>
          ))}
        </VStack>
      </Box>
    </ScrollView>
  );
}

import { type ReactNode, useState } from 'react';
import { Card } from '../ui/card';
import { Divider } from '../ui/divider';
import { HStack } from '../ui/hstack';
import { ChevronDownIcon, ChevronUpIcon, Icon } from '../ui/icon';
import { Pressable } from '../ui/pressable';
import { Text } from '../ui/text';
import { VStack } from '../ui/vstack';

export interface EntityListMobileCardField {
  key: string;
  column?: string;
  label?: string;
  value: ReactNode;
}

interface EntityListMobileCardProps {
  id: string | number;
  title: ReactNode;
  fields: EntityListMobileCardField[];
  collapsedFields?: EntityListMobileCardField[];
  checkbox?: ReactNode;
  menu?: ReactNode;
  className?: string;
  isSelected?: boolean;
  onPress?: () => void;
}

/** Отображение записи списковой формы в виде мобильной карточки. */
export default function EntityListMobileCard({
  id,
  title,
  fields,
  collapsedFields = [],
  checkbox,
  menu,
  className,
  isSelected = false,
  onPress,
}: EntityListMobileCardProps) {
  const [expandedId, setExpandedId] = useState<string | number | null>(null);
  const isExpanded = expandedId === id;
  const visibleFields = isExpanded ? [...fields, ...collapsedFields] : fields;
  const isExpandable = collapsedFields.length > 0;
  const cardClassName = [
    'gap-0 overflow-hidden p-0',
    isSelected ? 'border-primary bg-primary/10' : undefined,
    className,
  ].filter(Boolean).join(' ');

  return (
    <Card
      size="sm"
      className={cardClassName}
    >
      <HStack className="items-center gap-3 px-4 py-3">
        {checkbox}
        <Pressable
          className="min-w-0 flex-1"
          onPress={onPress}
          disabled={!onPress}
          accessibilityRole={onPress ? 'button' : undefined}
        >
          {title}
        </Pressable>
        {menu}
        {isExpandable && (
          <Pressable
            className="h-8 w-8 items-center justify-center rounded"
            onPress={() => setExpandedId(isExpanded ? null : id)}
            accessibilityRole="button"
            accessibilityState={{ expanded: isExpanded }}
            accessibilityLabel={
              isExpanded ? 'Свернуть карточку' : 'Развернуть карточку'
            }
          >
            <Icon
              as={isExpanded ? ChevronUpIcon : ChevronDownIcon}
              size="sm"
              className="text-foreground-secondary"
            />
          </Pressable>
        )}
      </HStack>
      {visibleFields.length > 0 && <Divider />}
      {visibleFields.length > 0 && (
        <Pressable
          onPress={onPress}
          disabled={!onPress}
          accessibilityRole={onPress ? 'button' : undefined}
        >
          <VStack className="gap-3 px-4 py-3">
            {visibleFields.map((field) => (
              <VStack key={field.key} className="gap-0.5">
                {field.label && (
                  <Text className="text-sm text-foreground-secondary">
                    {field.label}
                  </Text>
                )}
                {field.value}
              </VStack>
            ))}
          </VStack>
        </Pressable>
      )}
    </Card>
  );
}

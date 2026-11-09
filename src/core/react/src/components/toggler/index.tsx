import { type ElementType, type ReactNode, useCallback } from 'react';

import {
  Accordion,
  AccordionContent,
  AccordionHeader,
  AccordionIcon,
  AccordionItem,
  AccordionTitleText,
  AccordionTrigger,
} from '@tmobis/core/components/ui/accordion';
import { Box } from '@tmobis/core/components/ui/box';
import { ChevronDownIcon } from '@tmobis/core/components/ui/icon';
import { VStack } from '@tmobis/core/components/ui/vstack';

const TOGGLER_ITEM_VALUE = 'toggler';

type TogglerProps = {
  children: ReactNode;
  /** Заголовок блока */
  title?: string;
  name?: string;
  /** Раскрыт по умолчанию (неконтролируемый режим) */
  defaultExpanded?: boolean;
  /** Контролируемое состояние раскрытия */
  isExpanded?: boolean;
  isOpen?: boolean;
  /** Отключение взаимодействия */
  isDisabled?: boolean;
  /** Изменение состояния раскрытия */
  onExpandedChange?: (isExpanded: boolean) => void;
  /** Иконка индикатора (по умолчанию ChevronDownIcon, вращается при раскрытии) */
  icon?: ElementType;
  className?: string;
  contentClassName?: string;
  /**
   * Пунктирный разделитель между заголовком и иконкой
   * (визуальный паттерн legacy flexberry-toggler)
   */
  showDivider?: boolean;
};

/**
 * Скрываемый/раскрываемый блок контента.
 * Состояния: Collapsed, Expanded, Disabled.
 * Анимация высоты и иконки — через Accordion (react-native-reanimated).
 */
const Toggler = ({
  children,
  title,
  name,
  defaultExpanded,
  isExpanded,
  isOpen,
  isDisabled = false,
  onExpandedChange,
  icon = ChevronDownIcon,
  className,
  contentClassName,
  showDivider = false,
}: TogglerProps) => {
  const heading = title ?? name ?? '';
  const isControlled = isExpanded !== undefined;
  const resolvedDefaultExpanded = defaultExpanded ?? isOpen ?? false;

  const handleValueChange = useCallback(
    (values: string[]) => {
      onExpandedChange?.(values.includes(TOGGLER_ITEM_VALUE));
    },
    [onExpandedChange],
  );

  const accordionStateProps = isControlled
    ? {
        value: isExpanded ? [TOGGLER_ITEM_VALUE] : [],
        onValueChange: handleValueChange,
      }
    : {
        defaultValue: resolvedDefaultExpanded ? [TOGGLER_ITEM_VALUE] : [],
        onValueChange: handleValueChange,
      };

  return (
    <Box className={`w-full mb-6${className ? ` ${className}` : ''}`}>
      <Accordion
        type="single"
        isCollapsible={true}
        isDisabled={isDisabled}
        {...accordionStateProps}
      >
        <AccordionItem value={TOGGLER_ITEM_VALUE} isDisabled={isDisabled}>
          <AccordionHeader>
            <AccordionTrigger>
              {showDivider ? (
                <>
                  {heading ? (
                    <AccordionTitleText className="flex-none">
                      {heading}
                    </AccordionTitleText>
                  ) : null}
                  {/* Пунктир между заголовком и иконкой (как <hr> в legacy) */}
                  <Box className="flex-1 mx-4 h-px self-center border-b border-dashed border-muted-foreground/40" />
                  <AccordionIcon as={icon} />
                </>
              ) : (
                <>
                  <AccordionTitleText>{heading}</AccordionTitleText>
                  <AccordionIcon as={icon} />
                </>
              )}
            </AccordionTrigger>
          </AccordionHeader>
          <AccordionContent>
            <VStack
              space="sm"
              className={contentClassName}
            >
              {children}
            </VStack>
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </Box>
  );
};

Toggler.displayName = 'Toggler';

export { Toggler };
export type { TogglerProps };

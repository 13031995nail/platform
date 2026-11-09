import React, { type ReactNode } from 'react';

import { SlotOutlet } from '@tmobis/core/components/entity-edit/field-extension';
import { VStack } from '../ui/vstack';

import type { FormContentLayoutProps } from './form-content-layout';

function hasSlotContent(
  tabSlots: Map<string, ReactNode[]>,
  slotId: string,
): boolean {
  return (tabSlots.get(slotId)?.length ?? 0) > 0;
}

/**
 * Нативный лейаут содержимого вкладки: слоты левой и правой колонок
 * сохраняются, но выстраиваются одной колонкой сверху вниз.
 */
export function FormContentLayout({ tabSlots, slotTypes }: FormContentLayoutProps) {
  const stackedSlotIds = [
    slotTypes.headerTop,
    slotTypes.headerBottom,
    slotTypes.leftTop,
    slotTypes.leftBottom,
    slotTypes.rightTop,
    slotTypes.rightBottom,
    slotTypes.footerTop,
    slotTypes.footerBottom,
  ].filter(slotId => hasSlotContent(tabSlots, slotId));

  return (
    <VStack className="gap-4">
      {stackedSlotIds.map(slotId => (
        <VStack key={slotId} className="gap-4">
          <SlotOutlet
            tabSlots={tabSlots}
            slotId={slotId}
          />
        </VStack>
      ))}
    </VStack>
  );
}

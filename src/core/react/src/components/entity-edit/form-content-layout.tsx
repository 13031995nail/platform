import React, { type ReactNode } from 'react';

import { SlotOutlet } from '@tmobis/core/components/entity-edit/field-extension';
import { HStack } from '../ui/hstack';
import { VStack } from '../ui/vstack';

export type FormContentLayoutProps = {
  tabSlots: Map<string, ReactNode[]>;
  slotTypes: {
    headerTop: string;
    headerBottom: string;
    leftTop: string;
    leftBottom: string;
    rightTop: string;
    rightBottom: string;
    footerTop: string;
    footerBottom: string;
  };
};

/**
 * Веб-лейаут содержимого вкладки: шапка, две колонки, подвал.
 */
export function FormContentLayout({ tabSlots, slotTypes }: FormContentLayoutProps) {
  return (
    <>
      <VStack className="gap-4">
        <SlotOutlet
          tabSlots={tabSlots}
          slotId={slotTypes.headerTop}
        />
        <SlotOutlet
          tabSlots={tabSlots}
          slotId={slotTypes.headerBottom}
        />
      </VStack>

      <HStack className="flex-1 gap-4">
        <VStack className="flex-1 gap-4">
          <VStack className="flex-1">
            <SlotOutlet
              tabSlots={tabSlots}
              slotId={slotTypes.leftTop}
            />
          </VStack>
          <VStack>
            <SlotOutlet
              tabSlots={tabSlots}
              slotId={slotTypes.leftBottom}
            />
          </VStack>
        </VStack>

        <VStack className="flex-1 gap-4">
          <VStack className="flex-1">
            <SlotOutlet
              tabSlots={tabSlots}
              slotId={slotTypes.rightTop}
            />
          </VStack>
          <VStack>
            <SlotOutlet
              tabSlots={tabSlots}
              slotId={slotTypes.rightBottom}
            />
          </VStack>
        </VStack>
      </HStack>

      <VStack className="gap-4 pt-4">
        <SlotOutlet
          tabSlots={tabSlots}
          slotId={slotTypes.footerTop}
        />
        <SlotOutlet
          tabSlots={tabSlots}
          slotId={slotTypes.footerBottom}
        />
      </VStack>
    </>
  );
}

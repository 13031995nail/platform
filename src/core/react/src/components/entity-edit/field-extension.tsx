import {IEntity} from "@tmobis/core/models/entity";
import React, {ReactNode, Fragment} from "react";

type SlotRegion = 'leftColumn' | 'rightColumn' | 'header' | 'footer';

export type SlotId = {
  region: SlotRegion;
  position?: 'top' | 'bottom'
};

interface DefaultSlotTypes {
  headerTop: string;
  headerBottom: string;
  leftTop: string;
  leftBottom: string;
  rightTop: string;
  rightBottom: string;
  footerTop: string;
  footerBottom: string;
}

export const defaultSlots: DefaultSlotTypes = {
  headerTop: serializeSlot({region: "header", position: "top"}),
  headerBottom: serializeSlot({region: "header", position: "bottom"}),
  leftTop: serializeSlot({region: "leftColumn", position: "top"}),
  leftBottom: serializeSlot({region: "leftColumn", position: "bottom"}),
  rightTop: serializeSlot({region: "rightColumn", position: "top"}),
  rightBottom: serializeSlot({region: "rightColumn", position: "bottom"}),
  footerTop: serializeSlot({region: "footer", position: "top"}),
  footerBottom: serializeSlot({region: "footer", position: "bottom"})
}

export function serializeSlot(slotId: SlotId, tab?: string): string {
  const position = slotId.position ?? 'top';

  const tabStr = tab
    ? `${tab}:`
    : '';

  return `${tabStr}${slotId.region}:${position}`;
}

export default interface FieldExtension<TEntity = IEntity> {
  slot: string;
  route: string;
  tab?: string;
  order?: number;
  name: string;
  component: () => ReactNode;
  predicate?: (entity: TEntity | undefined) => boolean; // TODO: надо ли?
}

export interface FormSlotProps {
  slot: string;
  tab?: string;
  children: React.ReactNode;
}

export function FormSlot({ slot, children }: FormSlotProps) {
  return (
    <>
      {children}
    </>
  )
}
FormSlot.__isFormSlot = true as const;

export function SlotOutlet({
  slotId,
  tabSlots
}: {
  tabSlots: Map<string, ReactNode[]>;
  slotId: string;
}) {
  const elements = tabSlots.get(slotId) ?? [];
  return (
    <>
      {elements.map((element, index) => {
        if (React.isValidElement(element)) {
          return element.key ? element : React.cloneElement(element, { key: `${slotId}-${index}` });
        }
        return <Fragment key={`${slotId}-${index}`}>{element}</Fragment>
      })}
    </>
  );
}

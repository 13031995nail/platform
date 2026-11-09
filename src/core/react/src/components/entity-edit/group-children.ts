import { Children, isValidElement, type ReactNode } from 'react';

import type { CompositionRoot } from '@tmobis/core/modules/composition-root';
import {
  FormSlotProps,
  serializeSlot,
  type SlotId,
  defaultSlots,
} from '@tmobis/core/components/entity-edit/field-extension';

export const NO_TAB_ID = '__no-tab__';

export type TabSlotMap = Map<string, Map<string, ReactNode[]>>;

/**
 * Группировка дочерних элементов формы по вкладкам и слотам.
 */
export function groupChildrenBySlot(
  routeName: string,
  children: ReactNode,
  compositionRoot: CompositionRoot,
): TabSlotMap {
  const tabs: TabSlotMap = new Map();

  Children.forEach(children, child => {
    if (!isValidElement(child)) {
      return;
    }

    const type = child.type as { __isFormSlot?: true };

    let slotId: string;
    let slotComponent: ReactNode;
    let currentTabId: string = NO_TAB_ID;

    if (type?.__isFormSlot) {
      const { tab, slot: formSlot, children: formChildren } = child.props as FormSlotProps;
      slotComponent = formChildren;
      slotId = formSlot;
      if (tab) {
        currentTabId = tab;
      }
    } else {
      const defaultSlot: SlotId = { region: 'header', position: 'top' };
      slotId = serializeSlot(defaultSlot, currentTabId === NO_TAB_ID ? undefined : currentTabId);
      slotComponent = child;
    }

    if (!tabs.has(currentTabId)) {
      tabs.set(currentTabId, new Map());
    }
    const tabSlots = tabs.get(currentTabId)!;
    if (!tabSlots.has(slotId)) {
      tabSlots.set(slotId, []);
    }
    tabSlots.get(slotId)!.push(slotComponent);
  });

  const tabsWithExtensions = compositionRoot.getTabsWithExtensions(routeName);
  for (const [currentTabId, extensionsBySlot] of tabsWithExtensions) {
    if (!tabs.has(currentTabId)) {
      tabs.set(currentTabId, new Map());
    }
    const tabSlots = tabs.get(currentTabId)!;

    for (const [slotId, extensions] of extensionsBySlot) {
      if (!tabSlots.has(slotId)) {
        tabSlots.set(slotId, []);
      }

      tabSlots.get(slotId)!.push(
        ...extensions.map(ext => ext.component())
      );
    }
  }

  const filteredTabs: TabSlotMap = new Map();
  for (const [tabId, tabSlots] of tabs) {
    if (tabId === NO_TAB_ID || tabHasContent(tabSlots)) {
      filteredTabs.set(tabId, tabSlots);
    }
  }
  return filteredTabs;
}

export function tabHasContent(tabSlots: Map<string, ReactNode[]>): boolean {
  for (const slotId of Object.values(defaultSlots)) {
    const nodes = tabSlots.get(slotId);
    if (nodes && nodes.length > 0) {
      return true;
    }
  }
  return false;
}

export function shouldRenderTabs(tabs: TabSlotMap): boolean {
  const realTabCount = Array.from(tabs.keys()).filter(
    tabId => tabId !== NO_TAB_ID && tabHasContent(tabs.get(tabId) || new Map())
  ).length;
  return realTabCount >= 1;
}

export function getTabTitle(
  tabId: string,
  tabTitleMap?: Record<string, string>,
): string {
  if (tabTitleMap?.[tabId]) {
    return tabTitleMap[tabId];
  }
  if (tabId === NO_TAB_ID) {
    return 'Общее';
  }
  return tabId;
}

'use client';

import React, { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Tabs,
  TabsContent,
  TabsContentWrapper,
  TabsIndicator,
  TabsList,
  TabsTrigger,
  TabsTriggerIcon,
  TabsTriggerText,
} from '@tmobis/core/components/ui/tabs';
import type { TabBarProps } from './types';

export type { TabBarItem, TabBarProps } from './types';

function resolveInitialTab(
  tabs: TabBarProps['tabs'],
  defaultValue?: string
): string | undefined {
  if (tabs.length === 0) {
    return undefined;
  }
  if (defaultValue && tabs.some((t) => t.id === defaultValue)) {
    return defaultValue;
  }
  return tabs[0].id;
}

export const TabBar = ({
  tabs,
  value: valueProp,
  defaultValue,
  onValueChange,
  variant = 'underlined',
  orientation = 'horizontal',
  lazy = false,
  disabled = false,
  swipeEnabled = false,
}: TabBarProps) => {
  const initialTab = useMemo(
    () => resolveInitialTab(tabs, defaultValue),
    [tabs, defaultValue]
  );

  const [internalValue, setInternalValue] = useState<string | undefined>(initialTab);
  const [mountedIds, setMountedIds] = useState<Set<string>>(() => {
    const initial = resolveInitialTab(tabs, defaultValue);
    return initial ? new Set([initial]) : new Set();
  });

  const activeValue = valueProp ?? internalValue;

  const handleValueChange = useCallback(
    (next: string) => {
      onValueChange?.(next);
      if (valueProp === undefined) {
        setInternalValue(next);
      }
    },
    [onValueChange, valueProp]
  );

  const resolvedActive = useMemo(() => {
    if (activeValue && tabs.some((tab) => tab.id === activeValue)) {
      return activeValue;
    }
    return resolveInitialTab(tabs, defaultValue);
  }, [activeValue, tabs, defaultValue]);

  useEffect(() => {
    if (valueProp !== undefined || !resolvedActive) {
      return;
    }
    setInternalValue((prev) => (prev === resolvedActive ? prev : resolvedActive));
  }, [valueProp, resolvedActive]);

  useEffect(() => {
    if (swipeEnabled && process.env.NODE_ENV !== 'production') {
      console.warn(
        '[TabBar] swipeEnabled: в ui/tabs свайп между панелями не поддерживается. ' +
          'Используйте переключение по нажатию или подключите react-native-tab-view.'
      );
    }
  }, [swipeEnabled]);

  useEffect(() => {
    if (!initialTab) {
      return;
    }
    if (valueProp === undefined) {
      setInternalValue(initialTab);
    }
    setMountedIds((prev) => {
      const next = new Set(prev);
      next.add(initialTab);
      return next;
    });
  }, [initialTab, valueProp]);

  useEffect(() => {
    if (!resolvedActive) {
      return;
    }
    setMountedIds((prev) => {
      if (prev.has(resolvedActive)) {
        return prev;
      }
      const next = new Set(prev);
      next.add(resolvedActive);
      return next;
    });
  }, [resolvedActive]);

  if (tabs.length === 0 || !resolvedActive) {
    return null;
  }

  const shouldRenderContent = (tabId: string) => {
    if (!lazy) {
      return true;
    }
    return mountedIds.has(tabId);
  };

  return (
    <Tabs
      value={resolvedActive}
      onValueChange={handleValueChange}
      variant={variant}
      orientation={orientation}
      isDisabled={disabled}
    >
      <TabsList>
        <TabsIndicator />
        {tabs.map((tab) => (
          <TabsTrigger
            key={tab.id}
            value={tab.id}
            disabled={tab.disabled}
          >
            {tab.icon ? <TabsTriggerIcon as={tab.icon} /> : null}
            <TabsTriggerText>{tab.label}</TabsTriggerText>
          </TabsTrigger>
        ))}
      </TabsList>

      <TabsContentWrapper>
        {tabs.map((tab) =>
          shouldRenderContent(tab.id) ? (
            <TabsContent key={tab.id} value={tab.id}>
              {tab.content}
            </TabsContent>
          ) : null
        )}
      </TabsContentWrapper>
    </Tabs>
  );
};

export default TabBar;

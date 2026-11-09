import { useCallback, useRef, useState } from 'react';
import { KeyboardAvoidingView, Platform, type LayoutChangeEvent, type NativeScrollEvent, type NativeSyntheticEvent, type ScrollView as RNScrollView } from 'react-native';

import FormHeader from '@tmobis/core/components/form-header';
import { Box } from '../ui/box';
import { Heading } from '../ui/heading';
import { ScrollView } from '../ui/scroll-view';
import { VStack } from '../ui/vstack';

import type { FormChromeProps } from './form-chrome.types';
import { FormTabStrip } from './form-tab-strip';

/**
 * Нативная оболочка формы: вкладки, одноколоночный скролл, сохранение снизу.
 * При flattenTabs вкладки склеиваются в одно полотно, как useTabsAnchorMode в МИМК.
 * для отдельных вкладок flattenTabs={false}.
 */
export function FormChrome({
  isDirty,
  isSubmitting,
  submit,
  back,
  discardAndBack,
  saveError,
  flattenTabs = true,
  hasTabs,
  tabs,
}: FormChromeProps) {
  const [activeTabId, setActiveTabId] = useState(tabs[0]?.id);
  const scrollRef = useRef<RNScrollView>(null);
  const sectionOffsets = useRef<Record<string, number>>({});

  const stripTabs = tabs.map(tab => ({ id: tab.id, title: tab.title }));

  const handleTabChange = useCallback((tabId: string) => {
    setActiveTabId(tabId);
    if (!flattenTabs) {
      return;
    }
    const offsetY = sectionOffsets.current[tabId];
    if (offsetY == null) {
      return;
    }
    scrollRef.current?.scrollTo({ y: Math.max(offsetY - 8, 0), animated: true });
  }, [flattenTabs]);

  const handleScroll = useCallback((event: NativeSyntheticEvent<NativeScrollEvent>) => {
    if (!flattenTabs || tabs.length < 2) {
      return;
    }
    const y = event.nativeEvent.contentOffset.y + 24;
    let currentId = tabs[0].id;
    for (const tab of tabs) {
      const offset = sectionOffsets.current[tab.id];
      if (offset != null && offset <= y) {
        currentId = tab.id;
      }
    }
    setActiveTabId(prev => (prev === currentId ? prev : currentId));
  }, [flattenTabs, tabs]);

  const showStrip = hasTabs && tabs.length > 1;

  return (
    <KeyboardAvoidingView
      style={{ flex: 1 }}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
    >
      <VStack className="flex-1">
        {showStrip ? (
          <Box className="border-b border-border bg-background">
            <FormTabStrip
              tabs={stripTabs}
              value={activeTabId ?? tabs[0]?.id}
              onValueChange={handleTabChange}
            />
          </Box>
        ) : null}

        <ScrollView
          ref={scrollRef}
          style={{ flex: 1 }}
          keyboardShouldPersistTaps="handled"
          onScroll={flattenTabs ? handleScroll : undefined}
          scrollEventThrottle={16}
        >
          <VStack className="flex-1 px-4 pt-4 pb-6 gap-6">
            {flattenTabs || !hasTabs
              ? tabs.map(tab => (
                <VStack
                  key={tab.id}
                  className="gap-4"
                  onLayout={(event: LayoutChangeEvent) => {
                    sectionOffsets.current[tab.id] = event.nativeEvent.layout.y;
                  }}
                >
                  {hasTabs ? (
                    <Heading size="md">{tab.title}</Heading>
                  ) : null}
                  {tab.content}
                </VStack>
              ))
              : tabs.map(tab => (
                <Box
                  key={tab.id}
                  style={{ display: tab.id === activeTabId ? 'flex' : 'none' }}
                >
                  {tab.content}
                </Box>
              ))}
          </VStack>
        </ScrollView>

        <FormHeader
          isDirty={isDirty}
          isSubmitting={isSubmitting}
          submit={submit}
          back={back}
          discardAndBack={discardAndBack}
          saveError={saveError}
        />
      </VStack>
    </KeyboardAvoidingView>
  );
}

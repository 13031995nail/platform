import FormHeader from '@tmobis/core/components/form-header';
import { Heading } from '../ui/heading';
import { ScrollView } from '../ui/scroll-view';
import { Text } from '../ui/text';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../ui/tabs';
import { VStack } from '../ui/vstack';

import type { FormChromeProps } from './form-chrome.types';

export type { FormChromeProps, FormTabView } from './form-chrome.types';

/**
 * Веб-оболочка формы: шапка сверху, вкладки или сплошное полотно.
 */
export function FormChrome({
  isDirty,
  isSubmitting,
  submit,
  back,
  discardAndBack,
  saveError,
  flattenTabs = false,
  hasTabs,
  tabs,
}: FormChromeProps) {
  const horizontalPadding = 'px-6 md:px-8';
  const verticalPadding = 'pt-6';

  const flattenedCanvas = (
    <VStack className="gap-8">
      {tabs.map(tab => (
        <VStack key={tab.id} className="gap-4">
          {hasTabs ? (
            <Heading size="md">{tab.title}</Heading>
          ) : null}
          {tab.content}
        </VStack>
      ))}
    </VStack>
  );

  const tabbedCanvas = hasTabs
    ? (
      <Tabs defaultValue={tabs[0]?.id}>
        <TabsList>
          {tabs.map(tab => (
            <TabsTrigger key={tab.id} value={tab.id}>
              <Text>{tab.title}</Text>
            </TabsTrigger>
          ))}
        </TabsList>

        {tabs.map(tab => (
          <TabsContent key={tab.id} value={tab.id}>
            {tab.content}
          </TabsContent>
        ))}
      </Tabs>
    )
    : (tabs[0]?.content ?? null);

  return (
    <VStack className="flex-1">
      <FormHeader
        isDirty={isDirty}
        isSubmitting={isSubmitting}
        submit={submit}
        back={back}
        discardAndBack={discardAndBack}
        saveError={saveError}
      />

      <ScrollView style={{ flex: 1 }}>
        <VStack className={`flex-1 ${horizontalPadding} ${verticalPadding}`}>
          {flattenTabs ? flattenedCanvas : tabbedCanvas}
        </VStack>
      </ScrollView>
    </VStack>
  );
}

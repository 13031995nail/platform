import { Tabs, TabsIndicator, TabsList, TabsTrigger, TabsTriggerText } from '../ui/tabs';

export type FormTabStripItem = {
  id: string;
  title: string;
};

type FormTabStripProps = {
  tabs: FormTabStripItem[];
  value: string;
  onValueChange: (tabId: string) => void;
};

/**
 * Горизонтальная полоса вкладок формы.
 */
export function FormTabStrip({ tabs, value, onValueChange }: FormTabStripProps) {
  if (tabs.length === 0) {
    return null;
  }

  return (
    <Tabs
      value={value}
      onValueChange={onValueChange}
      variant="underlined"
    >
      <TabsList>
        <TabsIndicator />
        {tabs.map(tab => (
          <TabsTrigger key={tab.id} value={tab.id}>
            <TabsTriggerText>{tab.title}</TabsTriggerText>
          </TabsTrigger>
        ))}
      </TabsList>
    </Tabs>
  );
}

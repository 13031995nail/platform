import { useFocusEffect } from '@react-navigation/native';
import { TmobisSimpleDropdown } from '@tmobis/core/components/dropdown';
import {
  Accordion,
  AccordionContent,
  AccordionHeader,
  AccordionIcon,
  AccordionItem,
  AccordionTitleText,
  AccordionTrigger,
} from '@tmobis/core/components/ui/accordion';
import { ChevronDownIcon } from '@tmobis/core/components/ui/icon';
import { useHeader } from '@tmobis/core/components/header/context';
import { TmobisDropdownLookup } from '@tmobis/core/components/lookup';
import { useDropdownLookup } from '@tmobis/core/components/lookup/hook';
import type { EntityListData } from '@tmobis/core/components/entity-list/use-entity-list';
import type { ListFilterValues } from '@tmobis/core/api/filters';
import { Text } from '@tmobis/core/components/ui/text';
import { Town, TownClassName } from '@tmobis/core-business/models/town';
import { Redirect } from 'expo-router';
import { showComponentLab } from '@/constants/feature-flags';
import { useCallback, useMemo, useState, type ReactNode } from 'react';
import { Pressable, ScrollView, View } from 'react-native';

type DemoEntity = { id: string; name: string };
type DemoForm = { town?: Town };

const ENTITIES: DemoEntity[] = [
  { id: '1', name: 'Москва' },
  { id: '2', name: 'Санкт-Петербург' },
  { id: '3', name: 'Новосибирск' },
  { id: '4', name: 'Екатеринбург' },
  { id: '5', name: 'Казань' },
  { id: '6', name: 'Омск' },
  { id: '7', name: 'Томск' },
  { id: '8', name: 'Красноярск' },
];

const STATUSES = {
  active: 'Активен',
  paused: 'Приостановлен',
  archived: 'В архиве',
};

const entityLabel = (entity: DemoEntity) => entity.name;
const format = (value: unknown) => JSON.stringify(value);

function Section({
  title,
  description,
  children,
}: {
  title: string;
  description: string;
  children: ReactNode;
}) {
  return (
    <View className="gap-3 rounded-lg border border-outline p-4">
      <Text className="text-base font-semibold text-foreground">{title}</Text>
      <Text className="text-sm text-foreground-muted">{description}</Text>
      {children}
    </View>
  );
}

/** Независимая сворачиваемая категория каталога. */
function ComponentGroup({
  value,
  title,
  description,
  children,
}: {
  value: string;
  title: string;
  description: string;
  children: ReactNode;
}) {
  return (
    <AccordionItem value={value} className="rounded-lg border border-outline px-4 py-2">
      <AccordionHeader>
        <AccordionTrigger>
          <View className="flex-1 gap-1">
            <AccordionTitleText className="text-base font-semibold">{title}</AccordionTitleText>
            <Text className="text-xs text-foreground-muted">{description}</Text>
          </View>
          <AccordionIcon as={ChevronDownIcon} />
        </AccordionTrigger>
      </AccordionHeader>
      <AccordionContent className="pt-3">
        <View className="gap-3">{children}</View>
      </AccordionContent>
    </AccordionItem>
  );
}

/**
 * Локально имитирует серверную поисковую выдачу: старые options исчезают,
 * но выбранные значения должны оставаться в закрытом dropdown.
 */
function useSimulatedLookup(
  source: string,
  record: (source: string, event: string, value: unknown) => void
) {
  const [search, setSearch] = useState('');
  const applyFilterValues = useCallback(
    (_filter: ListFilterValues, nextSearch?: string) => {
      setSearch(nextSearch ?? '');
      record(source, 'onSearch', nextSearch ?? null);
    },
    [record, source]
  );

  const data = useMemo(() => {
    const query = search.trim().toLocaleLowerCase();
    return query
      ? ENTITIES.filter(({ name }) => name.toLocaleLowerCase().includes(query))
      : ENTITIES;
  }, [search]);

  // DropdownLookup использует из EntityListData только data/isLoading/filter.
  const listData = useMemo(() => ({
    data,
    isLoading: false,
    filter: {
      activeSearch: search || undefined,
      activeFilter: {},
      applyFilterValues,
    },
  }) as unknown as EntityListData<DemoEntity>, [applyFilterValues, data, search]);

  return { listData, search, count: data.length };
}

function ComponentLabContent() {
  const { setHeaderData } = useHeader();
  useFocusEffect(useCallback(() => {
    setHeaderData({ title: 'Тестирование компонентов' });
  }, [setHeaderData]));

  const [events, setEvents] = useState<string[]>([]);
  const record = useCallback((source: string, event: string, value: unknown) => {
    setEvents((old) => [
      `${source} · ${event}: ${format(value)}`,
      ...old,
    ].slice(0, 20));
  }, []);

  const singleLookup = useSimulatedLookup('lookup single', record);
  const multiLookup = useSimulatedLookup('lookup multi', record);
  const idLookup = useSimulatedLookup('lookup ID', record);
  const realTowns = useDropdownLookup<Town, DemoForm>({
    entityType: TownClassName,
    viewName: 'TownE',
  });

  const [singleEntity, setSingleEntity] = useState<DemoEntity | undefined>(ENTITIES[0]);
  const [multiEntities, setMultiEntities] = useState<DemoEntity[]>([ENTITIES[0]]);
  const [idOnly, setIdOnly] = useState<string | undefined>(ENTITIES[0].id);
  const [realTown, setRealTown] = useState<Town | undefined>();

  const [singleId, setSingleId] = useState<string | null>('1');
  const [multiIds, setMultiIds] = useState<string[] | null>(['1']);
  const [enumValue, setEnumValue] = useState<string | null>(STATUSES.active);
  const [enumValues, setEnumValues] = useState<string[] | null>([STATUSES.active]);

  return (
    <ScrollView
      className="bg-background"
      contentContainerStyle={{ padding: 16, gap: 16, paddingBottom: 48 }}
      keyboardShouldPersistTaps="handled"
    >
      <Text className="text-sm text-foreground-muted">
        Каталог для визуальной и функциональной проверки компонентов.
        Демонстрационные данные доступны без backend; пример TownE использует
        реальные запросы API. Группы раскрываются независимо.
      </Text>

      <Accordion type="multiple" defaultValue={['lookups']} className="gap-4">
        <ComponentGroup
          value="lookups"
          title="Лукапы"
          description="Связанные сущности: одиночный и множественный выбор, работа с ID и серверный поиск."
        >
          <Section
            title="Lookup single · серверный поиск (имитация)"
            description="Поиск от 2 символов, debounce 300 мс, выбранная сущность сохраняется при фильтрации."
          >
            <TmobisDropdownLookup
              listData={singleLookup.listData}
              displayField="name"
              label="Город (объект)"
              placeholder="Город"
              value={singleEntity}
              valueId={singleEntity?.id}
              onChange={(entity) => {
                setSingleEntity(entity);
                record('lookup single', 'onChange', entity);
              }}
              onChangeId={(id) => record('lookup single', 'onChangeId', id)}
            />
            <Text className="text-xs text-foreground-muted">
              Выбрано: {format({ id: singleEntity?.id, object: singleEntity })}; выдача: {singleLookup.count}
            </Text>
          </Section>

          <Section
            title="Lookup single · только ID"
            description="Проверка controlled valueId без onChange-объекта и без контекста EntityEdit."
          >
            <TmobisDropdownLookup
              listData={idLookup.listData}
              displayField="name"
              label="Город (ID)"
              placeholder="Город"
              valueId={idOnly}
              onChangeId={(id) => {
                setIdOnly(id);
                record('lookup ID', 'onChangeId', id);
              }}
            />
            <Text className="text-xs text-foreground-muted">
              valueId: {format(idOnly)}
            </Text>
          </Section>

          <Section
            title="Lookup multi · серверный поиск (имитация)"
            description="Сохранение выбора между поисковыми выдачами: Москва и Томск остаются выбранными при поиске Казани."
          >
            <TmobisDropdownLookup
              multiple
              listData={multiLookup.listData}
              displayField="name"
              label="Несколько городов"
              placeholder="Города"
              value={multiEntities}
              valueId={multiEntities.map(({ id }) => id)}
              onChange={(entities) => {
                setMultiEntities(entities);
                record('lookup multi', 'onChange', entities);
              }}
              onChangeId={(ids) => record('lookup multi', 'onChangeId', ids)}
            />
            <Text className="text-xs text-foreground-muted">
              Выбрано: {format(multiEntities)}; выдача: {multiLookup.count}
            </Text>
          </Section>

          <Section
            title="Lookup · реальный backend (TownE)"
            description="Запросы к API: debounce, includeTotalCount=false и сохранение подписи при изменении поисковой выдачи."
          >
            <TmobisDropdownLookup
              {...realTowns}
              displayField="name"
              label="Город из справочника"
              placeholder="Город"
              value={realTown}
              valueId={realTown?.id}
              onChange={(town) => {
                setRealTown(town);
                record('live TownE', 'onChange', town);
              }}
            />
            <Text className="text-xs text-foreground-muted">
              value: {format({ id: realTown?.id, name: realTown?.name })}
            </Text>
          </Section>
        </ComponentGroup>

        <ComponentGroup
          value="dropdowns"
          title="Дропдауны"
          description="Выпадающие списки: объекты и enum, single/multi, локальная фильтрация."
        >
          <Section title="SimpleDropdown · один элемент" description="Локальный поиск по массиву объектов в панели.">
            <TmobisSimpleDropdown
              options={ENTITIES}
              value={singleId}
              onChange={(id, entity) => {
                setSingleId(id);
                record('simple single', 'onChange', { id, entity });
              }}
              getOptionLabel={entityLabel}
              searchable
              searchPlacement="panel"
              minSearchLength={2}
              placeholder="Город"
            />
            <Text className="text-xs text-foreground-muted">value: {format(singleId)}</Text>
          </Section>

          <Section title="SimpleDropdown · несколько элементов" description="Локальный поиск и мультивыбор объектов.">
            <TmobisSimpleDropdown
              multiple
              options={ENTITIES}
              value={multiIds}
              onChange={(ids, entities) => {
                setMultiIds(ids);
                record('simple multi', 'onChange', { ids, entities });
              }}
              getOptionLabel={entityLabel}
              searchable
              searchPlacement="panel"
              minSearchLength={2}
              placeholder="Города"
            />
            <Text className="text-xs text-foreground-muted">value: {format(multiIds)}</Text>
          </Section>

          <Section title="SimpleDropdown · enum single" description="Обычный enum без поиска.">
            <TmobisSimpleDropdown
              options={STATUSES}
              value={enumValue}
              onChange={(value) => {
                setEnumValue(value);
                record('enum single', 'onChange', value);
              }}
              placeholder="Статус"
            />
            <Text className="text-xs text-foreground-muted">value: {format(enumValue)}</Text>
          </Section>

          <Section title="SimpleDropdown · enum multi" description="Несколько enum-значений.">
            <TmobisSimpleDropdown
              multiple
              options={STATUSES}
              value={enumValues}
              onChange={(value) => {
                setEnumValues(value);
                record('enum multi', 'onChange', value);
              }}
              placeholder="Статусы"
            />
            <Text className="text-xs text-foreground-muted">value: {format(enumValues)}</Text>
          </Section>
        </ComponentGroup>

        <ComponentGroup
          value="diagnostics"
          title="Диагностика"
          description="Журнал событий и значения обработчиков демонстрационных компонентов."
        >
          <Section title="События компонентов" description="Последние 20 вызовов onChange, onChangeId и onSearch.">
            <Pressable
              onPress={() => setEvents([])}
              accessibilityRole="button"
              className="self-start rounded-md border border-outline px-3 py-2"
            >
              <Text className="text-sm text-foreground">Очистить события</Text>
            </Pressable>
            {events.length === 0 ? (
              <Text className="text-sm text-foreground-muted">Событий пока нет</Text>
            ) : events.map((event, i) => (
              <Text key={`${i}:${event}`} className="text-xs text-foreground">
                {event}
              </Text>
            ))}
          </Section>
        </ComponentGroup>
      </Accordion>
    </ScrollView>
  );
}

export default function ComponentLab() {
  if (!showComponentLab) {
    return <Redirect href="/desktop" />;
  }
  return <ComponentLabContent />;
}

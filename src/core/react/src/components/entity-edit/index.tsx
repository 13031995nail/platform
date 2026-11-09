import { useMemo, type ReactNode } from 'react';
import { FieldValues, RegisterOptions } from 'react-hook-form';

import { IEntity } from '../../models/entity';
import { FormStateOverlay } from '../form-state';
import { useModules } from '@tmobis/core/modules/provider';
import { defaultSlots } from '@tmobis/core/components/entity-edit/field-extension';
import { EntityEditContext } from './context';
import { FormChrome } from './form-chrome';
import { FormContentLayout } from './form-content-layout';
import type { FormTabView } from './form-chrome.types';
import {
  getTabTitle,
  groupChildrenBySlot,
  shouldRenderTabs,
} from './group-children';
import type { UseEntityEditData } from './use-entity-edit';

export interface EditFormProps {
  isNew: boolean;
}

export interface EditFormFieldProps {
  fieldName?: string;
  label?: string;
  isRequired?: boolean;
  validationRules?: Omit<
    RegisterOptions<FieldValues, string>,
    | 'valueAsNumber'
    | 'valueAsDate'
    | 'setValueAs'
    | 'disabled'
  >;
}

export interface EntityEditTabMapping {
  [tabId: string]: string;
}

export interface EntityEditProps<
  TEntity extends IEntity & FieldValues
> extends UseEntityEditData<TEntity> {
  children: ReactNode;
  tabTitleMap?: EntityEditTabMapping;
  /**
   * Склейка вкладок в одно полотно (якоря + общий скролл), как useTabsAnchorMode в МИМК.
   * На нативе по умолчанию включена (через FormChrome.native); на web выключена.
   * false на native, если нужны отдельные вкладки.
   * Другой состав полей и вкладок через соседний `*.native.tsx` у маршрута формы.
   */
  flattenTabs?: boolean;
}

export default function EntityEdit<
  TEntity extends IEntity & FieldValues
>({
  formState,
  form,
  submit,
  back,
  discardAndBack,
  isDirty,
  isSubmitting,
  saveError,
  children,
  route,
  tabTitleMap,
  flattenTabs,
}: EntityEditProps<TEntity>) {
  const { compositionRoot } = useModules();
  const tabs = useMemo(
    () => groupChildrenBySlot(route, children, compositionRoot),
    [route, children, compositionRoot]
  );

  const hasTabs = shouldRenderTabs(tabs);

  const tabViews: FormTabView[] = useMemo(
    () => Array.from(tabs.entries()).map(([tabId, tabSlots]) => ({
      id: tabId,
      title: getTabTitle(tabId, tabTitleMap),
      content: (
        <FormContentLayout
          tabSlots={tabSlots}
          slotTypes={defaultSlots}
        />
      ),
    })),
    [tabs, tabTitleMap]
  );

  return (
    <EntityEditContext.Provider value={{ form }}>
      <FormStateOverlay formState={formState}>
        <FormChrome
          isDirty={isDirty}
          isSubmitting={isSubmitting}
          submit={submit}
          back={back}
          discardAndBack={discardAndBack}
          saveError={saveError}
          flattenTabs={flattenTabs}
          hasTabs={hasTabs}
          tabs={tabViews}
        />
      </FormStateOverlay>
    </EntityEditContext.Provider>
  );
}

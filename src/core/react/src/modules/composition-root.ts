import ModuleBase from "./module";
import initializeLocale from '../translations';
import {RequireContext} from "expo-router";
import ScreenData from "@tmobis/core/routes/screen-data";
import {DrawerGroupProps} from "@tmobis/core/components/drawer/drawer-group";
import FieldExtension, {serializeSlot, SlotId} from "@tmobis/core/components/entity-edit/field-extension";
import type {
  ListFormExtension,
  ListFormType,
} from "@tmobis/core/modules/list-form-extension";
import {createWebApiMessageProducer} from '../api/message-producer';
import {createMessageApi} from '../api/services/message-api';
import {createApiClient, type ApiClient} from '../api/create-api-client';
import Constants from 'expo-constants';
import {MessageDispatcher} from '../messages/dispatcher';
import type {ReactNode} from 'react';
import {
  DEFAULT_DESKTOP_LAYOUT,
  type DesktopItem,
  type DesktopLayoutConfig,
} from '@tmobis/core/desktop/types';
import { mergeSettingsItems } from '@tmobis/core/settings/group-items';
import type { SettingsItem } from '@tmobis/core/settings/types';
import { getCurrentUserRoles } from '@tmobis/core/security/permissions';

export function initializeModulesStatic(modules: ModuleBase[]) {
  initializeLocale(modules);

  for (const module of modules) {
    module.initStatic?.();
  }
}

export function getRouteImports(modules: ModuleBase[]): RequireContext[] {
  const ctx: RequireContext[] = [];

  for (const module of modules) {
    const imports = module.getRouteImports?.() ?? [];
    ctx.push(...imports);
  }

  return ctx;
}

export class CompositionRoot {
  modules: ModuleBase[];
  screens: ScreenData[] = [];
  groups: DrawerGroupProps[] = [];
  // Структура: fieldsByRoute[route][tab][slot] = extensions[]
  fieldsByRoute: Map<string, Map<string, Map<string, FieldExtension[]>>> = new Map();
  private readonly listFormExtensions: ListFormExtension[] = [];
  private readonly messageDispatcher: MessageDispatcher;
  private readonly apiClient: ApiClient;
  private shouldBeActive = false;
  private lifecycleTransition: Promise<void> = Promise.resolve();
  private readonly initializedModules = new Set<ModuleBase>();
  private readonly pendingCleanupModules = new Set<ModuleBase>();

  constructor(modules: ModuleBase[], client: ApiClient = createApiClient({
    baseURL: process.env.BACKEND_URL || Constants.expoConfig?.extra?.backendUrl,
  })) {
    this.modules = modules;
    this.apiClient = client;

    for (const module of modules) {
      module.configureApiClient?.(client);
    }

    this.initializeScreens();
    this.initializeGroups();
    this.initializeFieldExtensions();
    this.initializeListFormExtensions();
    this.messageDispatcher = new MessageDispatcher(
      this.modules.flatMap(
        (module) => module.getMessageProducers?.() ?? []
      ),
      createWebApiMessageProducer(createMessageApi(client))
    );
  }

  private initializeScreens() {
    for (const module of this.modules) {
      const moduleScreens = module.getScreens?.() ?? [];
      this.screens.push(...moduleScreens);
    }
  }

  private initializeGroups() {
    const groupMap = new Map<string, { routes: string[]; items: DrawerGroupProps['items'] }>();

    for (const screen of this.screens) {
      const group = screen.group;
      if (!group || screen.hideInNav) continue;

      if (!groupMap.has(group)) {
        groupMap.set(group, { routes: [], items: [] });
      }

      const groupData = groupMap.get(group)!;
      groupData.routes.push(screen.route);
    }

    for (const module of this.modules) {
      const drawerItems = module.getDrawerItems?.() ?? [];
      for (const { group, items } of drawerItems) {
        if (!groupMap.has(group)) {
          groupMap.set(group, { routes: [], items: [] });
        }

        groupMap.get(group)!.items?.push(...items);
      }
    }

    for (const [name, data] of groupMap) {
      this.groups.push({
        label: name,
        routes: data.routes,
        items: data?.items?.length ?? 0 > 0 ? data.items : undefined,
      });
    }
  }

  private initializeFieldExtensions() {
    for (const module of this.modules) {
      const extensions = module.getFieldExtensions?.() ?? [];

      for (const extension of extensions) {
        const route = extension.route;
        const tab = extension.tab ?? '__no-tab__';

        let byRoute = this.fieldsByRoute.get(route);
        if (!byRoute) {
          byRoute = new Map<string, Map<string, FieldExtension[]>>();
          this.fieldsByRoute.set(route, byRoute);
        }

        let byTab = byRoute.get(tab);
        if (!byTab) {
          byTab = new Map<string, FieldExtension[]>();
          byRoute.set(tab, byTab);
        }

        let list = byTab.get(extension.slot);
        if (!list) {
          list = [];
          byTab.set(extension.slot, list);
        }
        list.push(extension);
      }
    }

    for (const [, byRoute] of this.fieldsByRoute) {
      for (const [, byTab] of byRoute) {
        for (const [, list] of byTab) {
          list.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
        }
      }
    }
  }

  private initializeListFormExtensions() {
    for (const module of this.modules) {
      const extensions = module.getListFormExtensions?.() ?? [];
      this.listFormExtensions.push(...extensions);
    }
  }

  getScreens() {
    return {screens: this.screens, groups: this.groups}
  }

  
  /**
   * Разделение экранов для адаптивной навигации:
   * primary — нижние табы (апк), secondary — "Ещё" / полный drawer (веб).
   */
  getNavigationPlan(maxPrimaryTabs = 4) {
    const visible = this.screens.filter((screen) => !screen.hideInNav);

    const primary = visible
      .filter((screen) => screen.primary)
      .sort((a, b) => (a.priority ?? 99) - (b.priority ?? 99))
      .slice(0, maxPrimaryTabs);

    const primaryRoutes = new Set(primary.map((screen) => screen.route));
    const secondary = visible.filter((screen) => !primaryRoutes.has(screen.route));

    const secondaryRoutes = new Set(secondary.map((screen) => screen.route));
    const secondaryGroups = this.groups
      .map((group) => ({
        ...group,
        routes: (group.routes ?? []).filter((route) => secondaryRoutes.has(route)),
      }))
      .filter(
        (group) => (group.routes?.length ?? 0) > 0 || (group.items?.length ?? 0) > 0
      );

    return {
      screens: this.screens,
      groups: this.groups,
      primary,
      secondary,
      secondaryGroups,
    };
  }

  getTabsWithExtensions(route: string): Map<string, Map<string, FieldExtension[]>> {
    return this.fieldsByRoute.get(route) || new Map();
  }

  getListFormExtensions(listFormType: ListFormType) {
    return this.listFormExtensions.filter(
      extension => extension.listFormType === listFormType
    );
  }
  getApiClient(): ApiClient {
    return this.apiClient;
  }

  getMessageDispatcher(): MessageDispatcher {
    return this.messageDispatcher;
  }

  getHeaderActions(): ReactNode[] {
    return this.modules.flatMap(
      (module) => module.getHeaderActions?.() ?? []
    );
  }

  /**
   * Плитки рабочего стола: override первого модуля или merge вкладов модулей.
   */
  getDesktopItems(): DesktopItem[] {
    for (const module of this.modules) {
      const override = module.getDesktopItemsOverride?.();
      if (override !== undefined) {
        return filterDesktopItemsByRoles(override);
      }
    }

    const items = this.modules.flatMap(
      (module) => module.getDesktopItems?.() ?? []
    );

    return filterDesktopItemsByRoles(items).sort(
      (a, b) => (a.order ?? 0) - (b.order ?? 0)
    );
  }

  /**
   * Параметры сетки: defaults + merge getDesktopConfig() (later wins).
   */
  getDesktopLayout(): DesktopLayoutConfig {
    const layout: DesktopLayoutConfig = { ...DEFAULT_DESKTOP_LAYOUT };

    for (const module of this.modules) {
      const partial = module.getDesktopConfig?.();
      if (!partial) continue;
      if (partial.columns !== undefined) layout.columns = partial.columns;
      if (partial.iconSize !== undefined) layout.iconSize = partial.iconSize;
      if (partial.gap !== undefined) layout.gap = partial.gap;
    }

    return layout;
  }

  getSettingsItems(): SettingsItem[] {
    const items = this.modules.flatMap(
      (module) => module.getSettingsItems?.() ?? []
    );

    return filterSettingsItemsByRoles(mergeSettingsItems(items)).sort(
      (a, b) => {
        const groupDelta = (a.groupOrder ?? 0) - (b.groupOrder ?? 0);
        if (groupDelta !== 0) {
          return groupDelta;
        }
        return (a.order ?? 0) - (b.order ?? 0);
      }
    );
  }

  initialize(): Promise<void> {
    this.shouldBeActive = true;
    return this.enqueueLifecycleTransition();
  }

  dispose(): Promise<void> {
    this.shouldBeActive = false;
    return this.enqueueLifecycleTransition();
  }

  private enqueueLifecycleTransition(): Promise<void> {
    const transition = this.lifecycleTransition.then(
      () => this.reconcileLifecycle(),
      () => this.reconcileLifecycle()
    );
    this.lifecycleTransition = transition.catch(() => undefined);
    return transition;
  }

  private async reconcileLifecycle(): Promise<void> {
    if (this.shouldBeActive) {
      const errors: unknown[] = [];
      for (const module of this.modules) {
        if (
          this.initializedModules.has(module)
          || this.pendingCleanupModules.has(module)
        ) continue;
        try {
          await module.initialize?.({modules: this.modules, apiClient: this.apiClient});
          this.initializedModules.add(module);
        } catch (error) {
          errors.push(error);
          try {
            await module.dispose?.();
          } catch (disposeError) {
            errors.push(disposeError);
            this.pendingCleanupModules.add(module);
          }
        }
      }
      throwLifecycleErrors(errors, 'Module initialization failed');
      return;
    }

    const errors: unknown[] = [];
    for (const module of [...this.modules].reverse()) {
      if (
        !this.initializedModules.has(module)
        && !this.pendingCleanupModules.has(module)
      ) continue;
      try {
        await module.dispose?.();
        this.initializedModules.delete(module);
        this.pendingCleanupModules.delete(module);
      } catch (error) {
        errors.push(error);
      }
    }
    throwLifecycleErrors(errors, 'Module cleanup failed');
  }
}

function filterDesktopItemsByRoles(items: DesktopItem[]): DesktopItem[] {
  return filterItemsByRoles(items);
}

function filterSettingsItemsByRoles(items: SettingsItem[]): SettingsItem[] {
  return filterItemsByRoles(items);
}

function filterItemsByRoles<T extends { roles?: string[] }>(items: T[]): T[] {
  const userRoles = getCurrentUserRoles();

  return items.filter((item) => {
    if (!item.roles || item.roles.length === 0) {
      return true;
    }

    return item.roles.some((role) => userRoles.includes(role));
  });
}

function throwLifecycleErrors(errors: unknown[], message: string): void {
  if (errors.length === 1) throw errors[0];
  if (errors.length > 1) throw new AggregateError(errors, message);
}

import type {
  SettingsGroupSection,
  SettingsItem,
  SettingsItemVariant,
  SettingsSection,
} from './types';

/**
 * Группировка пунктов настроек одного варианта (грид или список строк).
 */
export function collectSettingsSections(
  items: SettingsItem[],
  variant: SettingsItemVariant,
): SettingsSection[] {
  return collectSettingsGroups(items)
    .map((section) => ({
      group: section.group,
      groupOrder: section.groupOrder,
      items: variant === 'action' ? section.actions : section.rows,
    }))
    .filter((section) => section.items.length > 0);
}

/**
 * Группировка пунктов настроек: в каждой группе сначала грид действий, затем строки.
 */
export function collectSettingsGroups(items: SettingsItem[]): SettingsGroupSection[] {
  const map = new Map<string, SettingsGroupSection>();

  for (const item of items) {
    const groupOrder = item.groupOrder ?? 0;
    let section = map.get(item.group);

    if (!section) {
      section = {
        group: item.group,
        groupOrder,
        actions: [],
        rows: [],
      };
      map.set(item.group, section);
    }

    section.groupOrder = Math.min(section.groupOrder, groupOrder);
    if (item.variant === 'action') {
      section.actions.push(item);
    } else {
      section.rows.push(item);
    }
  }

  for (const section of map.values()) {
    section.actions.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
    section.rows.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
  }

  return [...map.values()].sort((a, b) => {
    if (a.groupOrder !== b.groupOrder) {
      return a.groupOrder - b.groupOrder;
    }
    return a.group.localeCompare(b.group, 'ru');
  });
}

export function mergeSettingsItems(items: SettingsItem[]): SettingsItem[] {
  const byId = new Map<string, SettingsItem>();
  for (const item of items) {
    byId.set(item.id, item);
  }
  return [...byId.values()];
}

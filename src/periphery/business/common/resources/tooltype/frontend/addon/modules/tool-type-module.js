import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности типов инструментов.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class ToolTypeModuleBase extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('tool-types');
    router.route('tool-type',
    { path: 'tool-type/:id' });
    router.route('tool-type.new',
    { path: 'tool-type/new' });
  }

  getModuleLocales() {
    return {
      "ru": TranslationsRU,
      "en": TranslationsEN
    }
  }

  /**
   * Получить ноды сайтмапа данного модуля
   */
  getModuleSitemap(i18n) {
    return {
      dictionaries: [
        {
          link: 'tool-types',
          caption: i18n.t('forms.tool-types.caption'),
          title: i18n.t('forms.tool-types.caption'),
          icon: 'grid',
          children: null
        }
      ]
    }
  }

  /**
   * Получить ссылки в шапку сайта данного модуля
   */
  getModuleHeaderSitemap(i18n) {
  }
};

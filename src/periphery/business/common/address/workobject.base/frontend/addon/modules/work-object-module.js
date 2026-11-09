import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности объектов работ.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class WorkObjectModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('work-objects');
    router.route('work-object',
      { path: 'work-object/:id' });
    router.route('work-object.new',
      { path: 'work-object/new' });
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
          link: 'work-objects',
          caption: i18n.t('forms.work-object-l.caption'),
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

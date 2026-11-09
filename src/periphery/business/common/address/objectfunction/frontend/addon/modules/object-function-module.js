import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль справочника функций объектов.
 */
export class ObjectFunctionModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('object-functions');
    router.route('object-function',
      { path: 'object-function/:id' });
    router.route('object-function.new',
      { path: 'object-function/new' });
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
          link: 'object-functions',
          caption: i18n.t('forms.object-function-l.caption'),
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

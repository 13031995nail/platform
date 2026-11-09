import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

export class WorkTypeModuleBase extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('work-types');
    router.route('work-type',
    { path: 'work-type/:id' });
    router.route('work-type.new',
    { path: 'work-type/new' });
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
          link: 'work-types',
          caption: i18n.t('forms.work-types.caption'),
          title: i18n.t('forms.work-types.caption'),
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
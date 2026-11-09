import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности осенне-зимнего периода.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class AwpModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('awps');
    router.route('awp',
    { path: 'awp/:id' });
    router.route('awp.new',
    { path: 'awp/new' });
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
          link: 'awps',
          caption: i18n.t('forms.awps.caption'),
          title: i18n.t('forms.awps.caption'),
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

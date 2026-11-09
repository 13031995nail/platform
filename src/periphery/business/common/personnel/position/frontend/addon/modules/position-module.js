import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль справочника должностей.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class PositionModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('positions');
    router.route('position',
    { path: 'position/:id' });
    router.route('position.new',
    { path: 'position/new' });
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
          link: 'positions',
          caption: i18n.t('forms.positions.caption'),
          title: i18n.t('forms.positions.caption'),
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
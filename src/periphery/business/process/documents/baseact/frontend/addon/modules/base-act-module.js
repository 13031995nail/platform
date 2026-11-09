import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности актов.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class BaseActModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('documents');

    router.route('act-types');
    router.route('act-type',
      { path: 'act-type/:id' });
    router.route('act-type.new',
      { path: 'act-type/new' });

    router.route('act-statuses');
    router.route('act-status',
      { path: 'act-status/:id' });
    router.route('act-status.new',
      { path: 'act-status/new' });
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
  getModuleSitemap(i18n, userRoles) {
    return {
      dictionaries: [
        {
          link: 'act-types',
          caption: i18n.t('forms.act-type-l.caption'),
          roles: [userRoles.ADMIN]
        },
        {
          link: 'act-statuses',
          caption: i18n.t('forms.act-statuses.caption'),
          roles: [userRoles.ADMIN]
        }
      ]
    }
  }

  /**
   * Получить ссылки в шапку сайта данного модуля
   */
  getModuleHeaderSitemap(i18n) {
  }
}

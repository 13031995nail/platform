import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности подписей документов.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class ActSignModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('sign-types');
    router.route('sign-type',
      { path: 'sign-type/:id' });
    router.route('sign-type.new',
      { path: 'sign-type/new' });

    router.route('sign-roles');
    router.route('sign-role',
      { path: 'sign-role/:id' });
    router.route('sign-role.new',
      { path: 'sign-role/new' });
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
          link: 'sign-types',
          caption: i18n.t('forms.sign-types.caption'),
          roles: [userRoles.ADMIN]
        },
        {
          link: 'sign-roles',
          caption: i18n.t('forms.sign-roles.caption'),
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

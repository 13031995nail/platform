import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности организаций.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class OrganizationModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('organizations');
    router.route('organization',
      { path: 'organization/:id' });
    router.route('organization.new',
      { path: 'organization/new' });
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
          link: 'organizations',
          caption: i18n.t('forms.organization-l.caption'),
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

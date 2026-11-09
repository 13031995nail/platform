import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности типов организаций.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class OrganizationTypeModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('organization-types');
    router.route('organization-type',
    { path: 'organization-type/:id' });
    router.route('organization-type.new',
    { path: 'organization-type/new' });
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
          link: 'organization-types',
          caption: i18n.t('forms.organization-types.caption'),
          title: i18n.t('forms.organization-types.caption'),
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

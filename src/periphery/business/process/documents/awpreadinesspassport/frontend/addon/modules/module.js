import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности паспортов готовности ОЗП.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class AwpReadinessPassportModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('awp-readiness-passports');
    router.route('awp-readiness-passport',
      { path: 'awp-readiness-passport/:id' });
    router.route('awp-readiness-passport.new',
      { path: 'awp-readiness-passport/new' });
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
      documents: [
        {
          link: 'awp-readiness-passports',
          caption: i18n.t('forms.awp-readiness-passports.caption'),
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

import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности требований поверки.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class VerificationRequierementModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('verification-requierements');
    router.route('verification-requierement',
    { path: 'verification-requierement/:id' });
    router.route('verification-requierement.new',
    { path: 'verification-requierement/new' });
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
          link: 'verification-requierements',
          caption: i18n.t('forms.verification-requierements.caption'),
          title: i18n.t('forms.verification-requierements.caption'),
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
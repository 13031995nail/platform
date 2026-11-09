import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности актов готовности.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class RedinessActModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('rediness-acts');
    router.route('rediness-act',
      { path: 'rediness-act/:id' });
    router.route('rediness-act.new',
      { path: 'rediness-act/new' });
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
          link: 'rediness-acts',
          caption: i18n.t('forms.rediness-acts.caption'),
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

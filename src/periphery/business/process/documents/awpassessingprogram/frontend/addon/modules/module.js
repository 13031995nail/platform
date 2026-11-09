import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности программ оценки ОЗП.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class AwpAssessingProgramModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('awp-assessing-programs');
    router.route('awp-assessing-program',
      { path: 'awp-assessing-program/:id' });
    router.route('awp-assessing-program.new',
      { path: 'awp-assessing-program/new' });
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
          link: 'awp-assessing-programs',
          caption: i18n.t('forms.awp-assessing-programs.caption'),
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

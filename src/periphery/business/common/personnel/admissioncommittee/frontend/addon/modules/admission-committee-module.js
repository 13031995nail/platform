import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности приёмных комиссий.
 * Добавляет маршруты Ember, локали и ссылки шапки.
 */
export class AdmissionCommitteeModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('admission-committees');
    router.route('admission-committee',
      { path: 'admission-committee/:id' });
    router.route('admission-committee.new',
      { path: 'admission-committee/new' });
  }

  getModuleLocales() {
    return {
      "ru": TranslationsRU,
      "en": TranslationsEN
    }
  }

  /**
   * Получить ссылки в шапку сайта данного модуля
   */
  getModuleHeaderSitemap(i18n) {
  }
};

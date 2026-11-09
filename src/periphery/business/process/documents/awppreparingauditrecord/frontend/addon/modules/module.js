import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности журнала контроля подготовки ОЗП.
 * Добавляет маршруты Ember, локали и ссылки шапки.
 */
export class AwpPreparingAuditRecordModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route('work-object-remarks');
    router.route('awp-preparing-audit-records');
    router.route('awp-preparing-audit-record',
      { path: 'awp-preparing-audit-record/:id' });
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
}

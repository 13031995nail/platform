import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";
import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember модуль бизнес-аналитики.
 * Добавляет маршруты Ember, локали, узлы sitemap и ссылки шапки.
 */
export class BusinessIntelligenceModule extends ModuleBase {
  /**
   * Зарегистрировать маршруты модуля в роутере
   * @param {*} router - роутер для регистрации маршрутов
   */
  registerModuleRoutes(router) {
    router.route('tmobis-dashboards');
    router.route('tmobis-dashboard',
      { path: 'tmobis-dashboard/:id' });
      router.route('tmobis-dashboard.new',
      { path: 'tmobis-dashboard/new' });
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
      admin: [
        {
          link: 'tmobis-dashboards',
          caption: i18n.t('forms.dashboards.caption'),
          title: i18n.t('forms.dashboards.caption')
        }
      ]
    }
  }

  /**
   * Получить ссылки в шапку сайта данного модуля
   */
  getModuleHeaderSitemap(i18n) {
    return [];
  }
}
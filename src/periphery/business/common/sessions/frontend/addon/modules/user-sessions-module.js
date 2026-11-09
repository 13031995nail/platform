import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

/**
 * Ember бизнес-модуль сущности пользовательских сессий.
 * Добавляет маршруты Ember, узлы sitemap и ссылки шапки.
 */
export class UserSessionsModule extends ModuleBase {
  /**
   * Зарегистрировать маршруты модуля в роутере
   * @param {*} router - роутер для регистрации маршрутов
   */
  registerModuleRoutes(router) {
    router.route('user-sessions');
  }

  /**
   * Получить ноды сайтмапа данного модуля
   */
  getModuleSitemap(i18n) {
    return {
      admin: [
        {
          link: 'user-sessions',
          caption: i18n.t('forms.user-sessions.caption'),
          title: i18n.t('forms.user-sessions.caption')
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
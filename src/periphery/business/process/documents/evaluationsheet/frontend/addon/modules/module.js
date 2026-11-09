import { ModuleBase } from "@timepath/tmobis-core/modules/module-base";

import TranslationsRU from "../locales/ru/translations";
import TranslationsEN from "../locales/en/translations";

/**
 * Ember бизнес-модуль сущности оценочных листов.
 * Добавляет маршруты Ember, локали и узлы sitemap.
 */
export class EvaluationSheetModule extends ModuleBase {
  registerModuleRoutes(router) {
    router.route("evaluation-sheets");
    router.route("evaluation-sheet", { path: "evaluation-sheet/:id" });
    router.route("evaluation-sheet.new", { path: "evaluation-sheet/new" });
  }

  getModuleLocales() {
    return {
      ru: TranslationsRU,
      en: TranslationsEN
    };
  }

  getModuleSitemap(i18n) {
    return {
      documents: [
        {
          link: "evaluation-sheets",
          caption: i18n.t("forms.evaluation-sheets.caption")
        }
      ]
    };
  }
}

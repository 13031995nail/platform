import i18n, {Resource, ResourceLanguage} from "i18next";
import { initReactI18next } from "react-i18next";
import ModuleBase from "@tmobis/core/modules/module";

export default function initialize(modules: ModuleBase[]) {
  const resources: Resource = {};
  modules.forEach(module => {
    if (!module.locales) return; 
    for (const languageKey in module.locales) {
      const language = module.locales[languageKey];
      let existingLanguage = resources[languageKey];
      if (existingLanguage) {
        const existingTranslation : {} = existingLanguage.translation;
        const newTranslation : {} = language.translation;
        existingLanguage.translation = {...existingTranslation, ...newTranslation};
      } else {
        existingLanguage = language;
      }

      resources[languageKey] = existingLanguage;
    }
  });
  
  i18n.use(initReactI18next).init({
    resources,
    fallbackLng: "ru",
    interpolation: {
      escapeValue: false
    },
  });
};

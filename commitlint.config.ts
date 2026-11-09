import { UserConfig, RuleConfigSeverity } from "@commitlint/types";
export default {
   extends: ['@commitlint/config-conventional'],
   rules: {
      "subject-case": [RuleConfigSeverity.Error, "never",
            ["sentence-case", "start-case", "pascal-case", "upper-case"]],
      "body-max-line-length": [RuleConfigSeverity.Disabled],
      "scope-delimiter-style": [RuleConfigSeverity.Error, "always", ["/", ","]]
      // "scope-enum": [RuleConfigSeverity.Error, "always", ["бэк", "фронт"]],
   }
} satisfies UserConfig;
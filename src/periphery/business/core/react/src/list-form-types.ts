import { defineListFormType } from "@tmobis/core/modules/list-form-extension";

export const CoreBusinessListFormTypes = {
  Employee: defineListFormType("employee"),
  User: defineListFormType("user"),
} as const;

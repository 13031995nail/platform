import { defineListFormType } from "@tmobis/core/modules/list-form-extension";

export const ReportsListFormTypes = {
  Report: defineListFormType("report"),
} as const;

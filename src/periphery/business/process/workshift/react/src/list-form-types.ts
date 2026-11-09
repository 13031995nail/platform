import { defineListFormType } from "@tmobis/core/modules/list-form-extension";

export const WorkShiftListFormTypes = {
  WorkShift: defineListFormType("work-shift"),
  WorkShiftTask: defineListFormType("work-shift-task"),
} as const;

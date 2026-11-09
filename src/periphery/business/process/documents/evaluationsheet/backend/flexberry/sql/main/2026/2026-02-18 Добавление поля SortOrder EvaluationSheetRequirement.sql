--liquibase formatted sql
--changeset lprotasov:addEvaluationSheetRequirementSortOrder

ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS SortOrder INTEGER NOT NULL DEFAULT 0;

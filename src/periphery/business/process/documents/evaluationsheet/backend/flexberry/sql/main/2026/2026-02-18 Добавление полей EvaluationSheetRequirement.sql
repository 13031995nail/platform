--liquibase formatted sql
--changeset lprotasov:addEvaluationSheetRequirementFields

ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS reqOrder INTEGER NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS reqNumber VARCHAR(10) NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS shortReqName VARCHAR(2000) NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS indicatorName VARCHAR(2000) NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS weight DECIMAL(18,2) NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS isCalculated BOOLEAN NOT NULL DEFAULT false;
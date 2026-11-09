--liquibase formatted sql
--changeset portal:addRequirementHasEnteredValue

ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS HasEnteredValue BOOLEAN NOT NULL DEFAULT FALSE;
UPDATE EvaluationSheetRequirement SET HasEnteredValue = TRUE WHERE IsCalculated = TRUE AND CalculatedReq <> 0;

--changeset portal:addEvaluationSheetReviewRequestedAt
ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS ReviewRequestedAt TIMESTAMP NULL;

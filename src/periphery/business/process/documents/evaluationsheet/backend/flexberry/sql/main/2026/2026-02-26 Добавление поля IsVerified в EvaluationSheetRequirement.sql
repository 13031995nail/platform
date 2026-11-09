--liquibase formatted sql
--changeset lprotasov:addIsVerifiedToEvaluationSheetRequirement

ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS isVerified BOOLEAN NULL;

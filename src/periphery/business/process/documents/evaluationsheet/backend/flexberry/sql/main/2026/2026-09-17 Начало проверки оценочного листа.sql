--liquibase formatted sql
--changeset portal:addEvaluationSheetReviewStartedAt
ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS ReviewStartedAt TIMESTAMP NULL;

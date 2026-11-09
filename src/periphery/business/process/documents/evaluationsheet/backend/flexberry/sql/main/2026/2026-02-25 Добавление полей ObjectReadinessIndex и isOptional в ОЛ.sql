--liquibase formatted sql
--changeset nvaliullin:addEvaluationSheetObjectReadinessIndexIsOptional

ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS ObjectReadinessIndex DECIMAL(18,2) NULL;
ALTER TABLE EvaluationSheetRequirement ADD COLUMN IF NOT EXISTS isOptional BOOLEAN NOT NULL DEFAULT false;

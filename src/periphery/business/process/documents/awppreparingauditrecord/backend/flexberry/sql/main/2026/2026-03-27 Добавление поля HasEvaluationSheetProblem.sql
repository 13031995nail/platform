--liquibase formatted sql
--changeset KMenshchikova:addHasEvaluationSheetProblemToAwpPreparingAuditRecord

ALTER TABLE AwpPreparingAuditRecord ADD COLUMN IF NOT EXISTS HasEvaluationSheetProblem BOOLEAN NOT NULL DEFAULT false;

--liquibase formatted sql
--changeset lprotasov:addIsVerifiedTSOToEvaluationSheet

ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS isVerifiedTSO BOOLEAN NULL;

--liquibase formatted sql
--changeset nvaliullin:addOwnerlessObjectsToEvaluationSheet

ALTER TABLE EvaluationSheet ADD COLUMN IF NOT EXISTS OwnerlessObjects BOOLEAN NULL;
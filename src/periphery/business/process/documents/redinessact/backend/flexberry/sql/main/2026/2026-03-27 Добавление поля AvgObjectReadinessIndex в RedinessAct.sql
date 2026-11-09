--liquibase formatted sql
--changeset kmenshchikova:addAvgObjectReadinessIndexToRedinessAct

ALTER TABLE redinessact ADD COLUMN IF NOT EXISTS AvgObjectReadinessIndex DECIMAL(18,2) NULL;


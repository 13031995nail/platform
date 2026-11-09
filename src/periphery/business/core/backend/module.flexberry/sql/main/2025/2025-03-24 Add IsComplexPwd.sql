--liquibase formatted sql
--changeset SobakinDA:AddIsComplexPwdToUser

ALTER TABLE STORMAG ADD COLUMN IF NOT EXISTS IsComplexPwd BOOLEAN NULL;

--rollback ALTER TABLE STORMAG DROP COLUMN IF EXISTS IsComplexPwd;
--comment: Adding password complexity check to the user

--liquibase formatted sql
--changeset SobakinDA:AddAuditColumnsToSession

ALTER TABLE Session
ADD COLUMN CreateTime TIMESTAMP(3) NULL,
ADD COLUMN EditTime TIMESTAMP(3) NULL,
ADD COLUMN Creator VARCHAR(255) NULL,
ADD COLUMN Editor VARCHAR(255) NULL;

--rollback ALTER TABLE Session DROP COLUMN IF EXISTS CreateTime, DROP COLUMN IF EXISTS EditTime, DROP COLUMN IF EXISTS Creator, DROP COLUMN IF EXISTS Editor;
--comment: Adding audit fields CreateTime, EditTime, Creator, Editor to the Session table
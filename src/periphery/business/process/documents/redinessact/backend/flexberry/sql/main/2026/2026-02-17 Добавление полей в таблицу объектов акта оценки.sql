--liquibase formatted sql
--changeset kmenshchikova:AddExtFieldsToRedinessObject

ALTER TABLE redinessactobject ADD COLUMN IF NOT EXISTS number int NULL;
ALTER TABLE redinessactobject ADD COLUMN IF NOT EXISTS objectreadiness VARCHAR(20) NULL;

--rollback ALTER TABLE redinessactobject DROP COLUMN IF EXISTS Number; ALTER TABLE redinessactobject ALTER COLUMN objectReadiness TYPE VARCHAR(256);

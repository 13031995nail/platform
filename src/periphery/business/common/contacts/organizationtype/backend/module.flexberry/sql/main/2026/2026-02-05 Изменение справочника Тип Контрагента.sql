--liquibase formatted sql
--changeset mglukhov:2026-02-05-organization-type-addition

ALTER TABLE OrganizationType ALTER COLUMN Name TYPE VARCHAR(1500);
ALTER TABLE OrganizationType ADD COLUMN IF NOT EXISTS ShortName VARCHAR(250) NULL;

--rollback ALTER TABLE OrganizationType DROP COLUMN IF EXISTS ShortName; ALTER TABLE OrganizationType ALTER COLUMN Name TYPE VARCHAR(256);
--comment: Change column Name type to varchar 1500, add column ShortName varchar 250 